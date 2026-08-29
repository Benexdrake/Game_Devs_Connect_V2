using GameDevsConnect.Api.Infrastructure.Persistence;
using GameDevsConnect.Api.Modules.Social.Domain;
using GameDevsConnect.Api.Modules.Social.Events;
using GameDevsConnect.Api.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GameDevsConnect.Api.Modules.Social.Commands;

public record FollowUserCommand(string Username, Guid RequestingUserId) : IRequest<Result<bool>>;

public class FollowUserCommandHandler(AppDbContext db, IMediator mediator) : IRequestHandler<FollowUserCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(FollowUserCommand request, CancellationToken cancellationToken)
    {
        var target = await db.Users.FirstOrDefaultAsync(u => u.Username == request.Username, cancellationToken);
        if (target is null)
        {
            return Result<bool>.NotFound("User not found.");
        }

        if (target.Id == request.RequestingUserId)
        {
            return Result<bool>.ValidationError("You cannot follow yourself.");
        }

        var alreadyFollowing = await db.Follows.AnyAsync(
            f => f.FollowerUserId == request.RequestingUserId && f.TargetType == FollowTargetType.User && f.TargetId == target.Id,
            cancellationToken);
        if (alreadyFollowing)
        {
            return Result<bool>.Success(true);
        }

        db.Follows.Add(new Follow
        {
            Id = Guid.NewGuid(),
            FollowerUserId = request.RequestingUserId,
            TargetType = FollowTargetType.User,
            TargetId = target.Id,
            CreatedAt = DateTimeOffset.UtcNow,
        });
        await db.SaveChangesAsync(cancellationToken);

        await mediator.Publish(new UserFollowedEvent(target.Id, request.RequestingUserId), cancellationToken);

        return Result<bool>.Success(true);
    }
}

public record UnfollowUserCommand(string Username, Guid RequestingUserId) : IRequest<Result<bool>>;

public class UnfollowUserCommandHandler(AppDbContext db) : IRequestHandler<UnfollowUserCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(UnfollowUserCommand request, CancellationToken cancellationToken)
    {
        var target = await db.Users.FirstOrDefaultAsync(u => u.Username == request.Username, cancellationToken);
        if (target is null)
        {
            return Result<bool>.NotFound("User not found.");
        }

        var follow = await db.Follows.FirstOrDefaultAsync(
            f => f.FollowerUserId == request.RequestingUserId && f.TargetType == FollowTargetType.User && f.TargetId == target.Id,
            cancellationToken);
        if (follow is not null)
        {
            db.Follows.Remove(follow);
            await db.SaveChangesAsync(cancellationToken);
        }

        return Result<bool>.Success(true);
    }
}
