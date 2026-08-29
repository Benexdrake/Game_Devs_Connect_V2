using GameDevsConnect.Api.Infrastructure.Persistence;
using GameDevsConnect.Api.Modules.Contributions.Domain;
using GameDevsConnect.Api.Modules.Quests.Domain;
using GameDevsConnect.Api.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GameDevsConnect.Api.Modules.Contributions.Commands;

public record SubmissionLinkInput(string Url, string? Label);

public record CreateSubmissionCommand(
    Guid QuestId,
    Guid RequestingUserId,
    string Description,
    IReadOnlyList<SubmissionLinkInput>? Links) : IRequest<Result<SubmissionDto>>;

public class CreateSubmissionCommandHandler(AppDbContext db) : IRequestHandler<CreateSubmissionCommand, Result<SubmissionDto>>
{
    public async Task<Result<SubmissionDto>> Handle(CreateSubmissionCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Description))
        {
            return Result<SubmissionDto>.ValidationError("Description is required.");
        }

        var quest = await db.Quests.FirstOrDefaultAsync(q => q.Id == request.QuestId, cancellationToken);
        if (quest is null)
        {
            return Result<SubmissionDto>.NotFound("Quest not found.");
        }

        var hasActiveClaim = await db.QuestAssignments.AnyAsync(
            a => a.QuestId == quest.Id && a.UserId == request.RequestingUserId && a.ReleasedAt == null,
            cancellationToken);
        if (!hasActiveClaim)
        {
            return Result<SubmissionDto>.Forbidden("You need an active claim on this quest to submit.");
        }

        if (quest.Status != QuestStatus.InProgress)
        {
            return Result<SubmissionDto>.Conflict("Quest is not in a submittable state.");
        }

        var now = DateTimeOffset.UtcNow;
        var submission = new QuestSubmission
        {
            Id = Guid.NewGuid(),
            QuestId = quest.Id,
            UserId = request.RequestingUserId,
            Description = request.Description,
            Status = SubmissionStatus.PendingReview,
            SubmittedAt = now,
        };
        db.QuestSubmissions.Add(submission);

        foreach (var link in request.Links ?? [])
        {
            if (string.IsNullOrWhiteSpace(link.Url)) continue;

            db.SubmissionLinks.Add(new SubmissionLink
            {
                Id = Guid.NewGuid(),
                SubmissionId = submission.Id,
                Url = link.Url,
                Label = link.Label,
            });
        }

        quest.Status = QuestStatus.Submitted;
        quest.UpdatedAt = now;

        await db.SaveChangesAsync(cancellationToken);

        return Result<SubmissionDto>.Success(await SubmissionDtoBuilder.BuildAsync(db, submission, cancellationToken));
    }
}
