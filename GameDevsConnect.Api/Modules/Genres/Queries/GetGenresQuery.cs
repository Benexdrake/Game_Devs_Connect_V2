using GameDevsConnect.Api.Infrastructure.Persistence;
using GameDevsConnect.Api.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GameDevsConnect.Api.Modules.Genres.Queries;

public record GenreDto(Guid Id, string Name);

public record GetGenresQuery : IRequest<Result<IReadOnlyList<GenreDto>>>;

public class GetGenresQueryHandler(AppDbContext db)
    : IRequestHandler<GetGenresQuery, Result<IReadOnlyList<GenreDto>>>
{
    public async Task<Result<IReadOnlyList<GenreDto>>> Handle(GetGenresQuery request, CancellationToken cancellationToken)
    {
        var genres = await db.Genres
            .OrderBy(g => g.Name)
            .Select(g => new GenreDto(g.Id, g.Name))
            .ToListAsync(cancellationToken);

        return Result<IReadOnlyList<GenreDto>>.Success(genres);
    }
}
