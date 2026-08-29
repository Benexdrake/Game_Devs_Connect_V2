using GameDevsConnect.Api.Infrastructure.Persistence;
using GameDevsConnect.Api.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GameDevsConnect.Api.Modules.Engines.Queries;

public record EngineDto(Guid Id, string Name);

public record GetEnginesQuery : IRequest<Result<IReadOnlyList<EngineDto>>>;

public class GetEnginesQueryHandler(AppDbContext db)
    : IRequestHandler<GetEnginesQuery, Result<IReadOnlyList<EngineDto>>>
{
    public async Task<Result<IReadOnlyList<EngineDto>>> Handle(GetEnginesQuery request, CancellationToken cancellationToken)
    {
        var engines = await db.Engines
            .OrderBy(e => e.Name)
            .Select(e => new EngineDto(e.Id, e.Name))
            .ToListAsync(cancellationToken);

        return Result<IReadOnlyList<EngineDto>>.Success(engines);
    }
}
