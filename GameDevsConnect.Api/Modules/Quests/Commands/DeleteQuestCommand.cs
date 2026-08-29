using GameDevsConnect.Api.Infrastructure.Persistence;
using GameDevsConnect.Api.Modules.Projects;
using GameDevsConnect.Api.Modules.Projects.Domain;
using GameDevsConnect.Api.Modules.Quests.Domain;
using GameDevsConnect.Api.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GameDevsConnect.Api.Modules.Quests.Commands;

public record DeleteQuestCommand(string ProjectSlug, Guid QuestId, Guid RequestingUserId) : IRequest<Result<bool>>;

public class DeleteQuestCommandHandler(AppDbContext db) : IRequestHandler<DeleteQuestCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(DeleteQuestCommand request, CancellationToken cancellationToken)
    {
        var project = await db.Projects.FirstOrDefaultAsync(p => p.Slug == request.ProjectSlug, cancellationToken);
        if (project is null)
        {
            return Result<bool>.NotFound("Project not found.");
        }

        var quest = await db.Quests.FirstOrDefaultAsync(q => q.Id == request.QuestId && q.ProjectId == project.Id, cancellationToken);
        if (quest is null)
        {
            return Result<bool>.NotFound("Quest not found.");
        }

        var role = await ProjectAccess.GetRoleAsync(db, project.Id, request.RequestingUserId, cancellationToken);
        if (role is not (ProjectRole.Owner or ProjectRole.Admin))
        {
            return Result<bool>.Forbidden("Only the owner or an admin can delete this quest.");
        }

        var everClaimed = await db.QuestAssignments.AnyAsync(a => a.QuestId == quest.Id, cancellationToken);
        if (everClaimed)
        {
            quest.Status = QuestStatus.Cancelled;
            quest.UpdatedAt = DateTimeOffset.UtcNow;
        }
        else
        {
            db.Quests.Remove(quest);
        }

        await db.SaveChangesAsync(cancellationToken);
        return Result<bool>.Success(true);
    }
}
