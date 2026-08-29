using GameDevsConnect.Api.Infrastructure.Persistence;
using GameDevsConnect.Api.Modules.Skills.Domain;
using GameDevsConnect.Api.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GameDevsConnect.Api.Modules.Skills.Queries;

public record SkillDto(Guid Id, string Name, SkillCategory Category);

public record GetSkillsQuery : IRequest<Result<IReadOnlyList<SkillDto>>>;

public class GetSkillsQueryHandler(AppDbContext db)
    : IRequestHandler<GetSkillsQuery, Result<IReadOnlyList<SkillDto>>>
{
    public async Task<Result<IReadOnlyList<SkillDto>>> Handle(GetSkillsQuery request, CancellationToken cancellationToken)
    {
        var skills = await db.Skills
            .OrderBy(s => s.Category).ThenBy(s => s.Name)
            .Select(s => new SkillDto(s.Id, s.Name, s.Category))
            .ToListAsync(cancellationToken);

        return Result<IReadOnlyList<SkillDto>>.Success(skills);
    }
}
