using GameDevsConnect.Api.Infrastructure.Persistence;
using GameDevsConnect.Api.Modules.Projects;
using GameDevsConnect.Api.Modules.Projects.Domain;
using GameDevsConnect.Api.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GameDevsConnect.Api.Modules.Contributions.Queries;

public record GetQuestSubmissionsQuery(Guid QuestId, Guid RequestingUserId) : IRequest<Result<IReadOnlyList<SubmissionDto>>>;

public class GetQuestSubmissionsQueryHandler(AppDbContext db)
    : IRequestHandler<GetQuestSubmissionsQuery, Result<IReadOnlyList<SubmissionDto>>>
{
    public async Task<Result<IReadOnlyList<SubmissionDto>>> Handle(GetQuestSubmissionsQuery request, CancellationToken cancellationToken)
    {
        var quest = await db.Quests.FirstOrDefaultAsync(q => q.Id == request.QuestId, cancellationToken);
        if (quest is null)
        {
            return Result<IReadOnlyList<SubmissionDto>>.NotFound("Quest not found.");
        }

        var role = await ProjectAccess.GetRoleAsync(db, quest.ProjectId, request.RequestingUserId, cancellationToken);
        var canManage = role is ProjectRole.Owner or ProjectRole.Admin;

        var query = db.QuestSubmissions.Where(s => s.QuestId == quest.Id);
        if (!canManage)
        {
            query = query.Where(s => s.UserId == request.RequestingUserId);
        }

        var submissions = await query.OrderByDescending(s => s.SubmittedAt).ToListAsync(cancellationToken);

        var dtos = new List<SubmissionDto>(submissions.Count);
        foreach (var submission in submissions)
        {
            dtos.Add(await SubmissionDtoBuilder.BuildAsync(db, submission, cancellationToken));
        }

        return Result<IReadOnlyList<SubmissionDto>>.Success(dtos);
    }
}
