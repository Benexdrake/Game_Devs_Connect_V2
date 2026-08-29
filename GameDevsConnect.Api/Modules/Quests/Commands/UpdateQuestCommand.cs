using GameDevsConnect.Api.Infrastructure.Persistence;
using GameDevsConnect.Api.Modules.Projects;
using GameDevsConnect.Api.Modules.Projects.Domain;
using GameDevsConnect.Api.Modules.Quests.Domain;
using GameDevsConnect.Api.Modules.Skills.Domain;
using GameDevsConnect.Api.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GameDevsConnect.Api.Modules.Quests.Commands;

public record UpdateQuestCommand(
    string ProjectSlug,
    Guid QuestId,
    Guid RequestingUserId,
    string? Title,
    string? Description,
    SkillCategory? Category,
    QuestDifficulty? Difficulty,
    DateTimeOffset? Deadline,
    int? MaxContributors,
    IReadOnlyList<Guid>? RequiredSkillIds) : IRequest<Result<QuestDto>>;

public class UpdateQuestCommandHandler(AppDbContext db) : IRequestHandler<UpdateQuestCommand, Result<QuestDto>>
{
    public async Task<Result<QuestDto>> Handle(UpdateQuestCommand request, CancellationToken cancellationToken)
    {
        var project = await db.Projects.FirstOrDefaultAsync(p => p.Slug == request.ProjectSlug, cancellationToken);
        if (project is null)
        {
            return Result<QuestDto>.NotFound("Project not found.");
        }

        var quest = await db.Quests.FirstOrDefaultAsync(q => q.Id == request.QuestId && q.ProjectId == project.Id, cancellationToken);
        if (quest is null)
        {
            return Result<QuestDto>.NotFound("Quest not found.");
        }

        var role = await ProjectAccess.GetRoleAsync(db, project.Id, request.RequestingUserId, cancellationToken);
        if (role is not (ProjectRole.Owner or ProjectRole.Admin))
        {
            return Result<QuestDto>.Forbidden("Only the owner or an admin can edit this quest.");
        }

        if (quest.Status != QuestStatus.Open)
        {
            return Result<QuestDto>.Conflict("Quest can only be edited while it is open.");
        }

        if (request.Title is not null) quest.Title = request.Title;
        if (request.Description is not null) quest.Description = request.Description;
        if (request.Category is not null) quest.Category = request.Category.Value;
        if (request.Difficulty is not null)
        {
            quest.Difficulty = request.Difficulty.Value;
            quest.XpReward = QuestDifficultyXp.For(request.Difficulty.Value);
        }
        if (request.Deadline is not null) quest.Deadline = request.Deadline;
        if (request.MaxContributors is > 0) quest.MaxContributors = request.MaxContributors.Value;

        if (request.RequiredSkillIds is not null)
        {
            var existing = await db.QuestSkills.Where(qs => qs.QuestId == quest.Id).ToListAsync(cancellationToken);
            db.QuestSkills.RemoveRange(existing);
            foreach (var skillId in request.RequiredSkillIds.Distinct())
            {
                db.QuestSkills.Add(new QuestSkill { QuestId = quest.Id, SkillId = skillId });
            }
        }

        quest.UpdatedAt = DateTimeOffset.UtcNow;
        await db.SaveChangesAsync(cancellationToken);

        return Result<QuestDto>.Success(await QuestDtoBuilder.BuildAsync(db, quest, cancellationToken));
    }
}
