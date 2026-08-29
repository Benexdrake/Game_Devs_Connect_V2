using GameDevsConnect.Api.Infrastructure.Persistence;
using GameDevsConnect.Api.Modules.Projects;
using GameDevsConnect.Api.Modules.Projects.Domain;
using GameDevsConnect.Api.Modules.Quests.Domain;
using GameDevsConnect.Api.Modules.Skills.Domain;
using GameDevsConnect.Api.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GameDevsConnect.Api.Modules.Quests.Commands;

public record CreateQuestCommand(
    string ProjectSlug,
    Guid RequestingUserId,
    string Title,
    string? Description,
    SkillCategory Category,
    QuestDifficulty Difficulty,
    DateTimeOffset? Deadline,
    int? MaxContributors,
    IReadOnlyList<Guid>? RequiredSkillIds) : IRequest<Result<QuestDto>>;

public class CreateQuestCommandHandler(AppDbContext db) : IRequestHandler<CreateQuestCommand, Result<QuestDto>>
{
    public async Task<Result<QuestDto>> Handle(CreateQuestCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Title))
        {
            return Result<QuestDto>.ValidationError("Title is required.");
        }

        var project = await db.Projects.FirstOrDefaultAsync(p => p.Slug == request.ProjectSlug, cancellationToken);
        if (project is null)
        {
            return Result<QuestDto>.NotFound("Project not found.");
        }

        var role = await ProjectAccess.GetRoleAsync(db, project.Id, request.RequestingUserId, cancellationToken);
        if (role is not (ProjectRole.Owner or ProjectRole.Admin))
        {
            return Result<QuestDto>.Forbidden("Only the owner or an admin can create quests for this project.");
        }

        var now = DateTimeOffset.UtcNow;
        var quest = new Quest
        {
            Id = Guid.NewGuid(),
            ProjectId = project.Id,
            CreatorId = request.RequestingUserId,
            Title = request.Title,
            Description = request.Description,
            Category = request.Category,
            Difficulty = request.Difficulty,
            XpReward = QuestDifficultyXp.For(request.Difficulty),
            Status = QuestStatus.Open,
            Deadline = request.Deadline,
            MaxContributors = request.MaxContributors is > 0 ? request.MaxContributors.Value : 1,
            CreatedAt = now,
            UpdatedAt = now,
        };
        db.Quests.Add(quest);

        foreach (var skillId in (request.RequiredSkillIds ?? []).Distinct())
        {
            db.QuestSkills.Add(new QuestSkill { QuestId = quest.Id, SkillId = skillId });
        }

        await db.SaveChangesAsync(cancellationToken);

        return Result<QuestDto>.Success(await QuestDtoBuilder.BuildAsync(db, quest, cancellationToken));
    }
}
