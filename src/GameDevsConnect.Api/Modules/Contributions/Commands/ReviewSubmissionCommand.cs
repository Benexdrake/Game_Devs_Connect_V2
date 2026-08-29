using GameDevsConnect.Api.Infrastructure.Persistence;
using GameDevsConnect.Api.Modules.Contributions.Domain;
using GameDevsConnect.Api.Modules.Projects;
using GameDevsConnect.Api.Modules.Projects.Domain;
using GameDevsConnect.Api.Modules.Quests.Domain;
using GameDevsConnect.Api.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GameDevsConnect.Api.Modules.Contributions.Commands;

public enum SubmissionDecision
{
    Accept,
    Reject,
    RequestChanges,
}

public record ReviewSubmissionCommand(
    Guid SubmissionId,
    Guid RequestingUserId,
    SubmissionDecision Decision,
    string? Comment) : IRequest<Result<SubmissionDto>>;

public class ReviewSubmissionCommandHandler(AppDbContext db) : IRequestHandler<ReviewSubmissionCommand, Result<SubmissionDto>>
{
    public async Task<Result<SubmissionDto>> Handle(ReviewSubmissionCommand request, CancellationToken cancellationToken)
    {
        var submission = await db.QuestSubmissions.FirstOrDefaultAsync(s => s.Id == request.SubmissionId, cancellationToken);
        if (submission is null)
        {
            return Result<SubmissionDto>.NotFound("Submission not found.");
        }

        var quest = await db.Quests.FirstAsync(q => q.Id == submission.QuestId, cancellationToken);
        var project = await db.Projects.FirstAsync(p => p.Id == quest.ProjectId, cancellationToken);

        var role = await ProjectAccess.GetRoleAsync(db, project.Id, request.RequestingUserId, cancellationToken);
        if (role is not (ProjectRole.Owner or ProjectRole.Admin))
        {
            return Result<SubmissionDto>.Forbidden("Only the owner or an admin can review submissions.");
        }

        if (submission.Status != SubmissionStatus.PendingReview)
        {
            return Result<SubmissionDto>.Conflict("Submission has already been reviewed.");
        }

        var now = DateTimeOffset.UtcNow;
        submission.ReviewedAt = now;
        submission.ReviewerId = request.RequestingUserId;
        submission.ReviewComment = request.Comment;
        quest.UpdatedAt = now;

        switch (request.Decision)
        {
            case SubmissionDecision.RequestChanges:
                submission.Status = SubmissionStatus.ChangesRequested;
                quest.Status = QuestStatus.InProgress;
                break;

            case SubmissionDecision.Reject:
                submission.Status = SubmissionStatus.Rejected;
                quest.Status = QuestStatus.Open;

                var activeAssignment = await db.QuestAssignments.FirstOrDefaultAsync(
                    a => a.QuestId == quest.Id && a.UserId == submission.UserId && a.ReleasedAt == null,
                    cancellationToken);
                if (activeAssignment is not null)
                {
                    activeAssignment.ReleasedAt = now;
                }
                break;

            case SubmissionDecision.Accept:
                submission.Status = SubmissionStatus.Accepted;
                quest.Status = QuestStatus.Accepted;

                db.Contributions.Add(new Contribution
                {
                    Id = Guid.NewGuid(),
                    UserId = submission.UserId,
                    ProjectId = project.Id,
                    QuestId = quest.Id,
                    SubmissionId = submission.Id,
                    CreatedAt = now,
                });
                // TODO Phase 4: award XP for this accepted contribution.
                break;
        }

        await db.SaveChangesAsync(cancellationToken);

        return Result<SubmissionDto>.Success(await SubmissionDtoBuilder.BuildAsync(db, submission, cancellationToken));
    }
}
