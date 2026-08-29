using GameDevsConnect.Api.Infrastructure.Persistence;
using GameDevsConnect.Api.Modules.Users.Domain;
using GameDevsConnect.Api.Modules.Users.Queries;
using GameDevsConnect.Api.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GameDevsConnect.Api.Modules.Users.Commands;

public record UserLinkInput(string Label, string Url);

public record UpdateUserProfileCommand(
    Guid UserId,
    string? Bio,
    IReadOnlyList<UserLinkInput>? Links) : IRequest<Result<UserProfileDto>>;

public class UpdateUserProfileCommandHandler(AppDbContext db, IMediator mediator)
    : IRequestHandler<UpdateUserProfileCommand, Result<UserProfileDto>>
{
    public async Task<Result<UserProfileDto>> Handle(UpdateUserProfileCommand request, CancellationToken cancellationToken)
    {
        var user = await db.Users.FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);
        if (user is null)
        {
            return Result<UserProfileDto>.NotFound("User not found.");
        }

        if (request.Bio is not null)
        {
            user.Bio = request.Bio;
        }

        if (request.Links is not null)
        {
            var existingLinks = await db.UserLinks.Where(l => l.UserId == user.Id).ToListAsync(cancellationToken);
            db.UserLinks.RemoveRange(existingLinks);

            foreach (var link in request.Links)
            {
                db.UserLinks.Add(new UserLink { Id = Guid.NewGuid(), UserId = user.Id, Label = link.Label, Url = link.Url });
            }
        }

        user.UpdatedAt = DateTimeOffset.UtcNow;
        await db.SaveChangesAsync(cancellationToken);

        return await mediator.Send(new GetUserProfileQuery(user.Username, user.Id), cancellationToken);
    }
}
