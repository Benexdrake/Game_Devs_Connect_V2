using GameDevsConnect.Api.Infrastructure.Persistence;
using GameDevsConnect.Api.Modules.Users.Domain;
using GameDevsConnect.Api.Modules.Users.Queries;
using GameDevsConnect.Api.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GameDevsConnect.Api.Modules.Users.Commands;

public record SetUserSkillsCommand(Guid UserId, IReadOnlyList<Guid> SkillIds) : IRequest<Result<IReadOnlyList<UserSkillDto>>>;

public class SetUserSkillsCommandHandler(AppDbContext db)
    : IRequestHandler<SetUserSkillsCommand, Result<IReadOnlyList<UserSkillDto>>>
{
    public async Task<Result<IReadOnlyList<UserSkillDto>>> Handle(SetUserSkillsCommand request, CancellationToken cancellationToken)
    {
        var requestedIds = request.SkillIds.Distinct().ToList();
        var existingSkills = await db.Skills.Where(s => requestedIds.Contains(s.Id)).ToListAsync(cancellationToken);
        if (existingSkills.Count != requestedIds.Count)
        {
            return Result<IReadOnlyList<UserSkillDto>>.ValidationError("One or more skill ids do not exist.");
        }

        var current = await db.UserSkills.Where(us => us.UserId == request.UserId).ToListAsync(cancellationToken);
        db.UserSkills.RemoveRange(current);

        foreach (var skillId in requestedIds)
        {
            db.UserSkills.Add(new UserSkill { UserId = request.UserId, SkillId = skillId });
        }

        await db.SaveChangesAsync(cancellationToken);

        var dtos = existingSkills.Select(s => new UserSkillDto(s.Id, s.Name, s.Category)).ToList();
        return Result<IReadOnlyList<UserSkillDto>>.Success(dtos);
    }
}
