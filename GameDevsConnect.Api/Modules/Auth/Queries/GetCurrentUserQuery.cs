using GameDevsConnect.Api.Infrastructure.Persistence;
using GameDevsConnect.Api.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GameDevsConnect.Api.Modules.Auth.Queries;

public record CurrentUserDto(Guid Id, string Username, string? AvatarUrl);

public record GetCurrentUserQuery(Guid UserId) : IRequest<Result<CurrentUserDto>>;

public class GetCurrentUserQueryHandler(AppDbContext db)
    : IRequestHandler<GetCurrentUserQuery, Result<CurrentUserDto>>
{
    public async Task<Result<CurrentUserDto>> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
    {
        var user = await db.Users
            .Where(u => u.Id == request.UserId)
            .Select(u => new CurrentUserDto(u.Id, u.Username, u.AvatarUrl))
            .FirstOrDefaultAsync(cancellationToken);

        return user is null
            ? Result<CurrentUserDto>.NotFound("User not found.")
            : Result<CurrentUserDto>.Success(user);
    }
}
