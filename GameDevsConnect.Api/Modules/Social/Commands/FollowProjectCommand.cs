using GameDevsConnect.Api.Infrastructure.Persistence;
using GameDevsConnect.Api.Modules.Social.Domain;
using GameDevsConnect.Api.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GameDevsConnect.Api.Modules.Social.Commands;

public record FollowProjectCommand(string Slug, Guid RequestingUserId) : IRequest<Result<bool>>;

public class FollowProjectCommandHandler(AppDbContext db) : IRequestHandler<FollowProjectCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(FollowProjectCommand request, CancellationToken cancellationToken)
    {
        var project = await db.Projects.FirstOrDefaultAsync(p => p.Slug == request.Slug, cancellationToken);
        if (project is null)
        {
            return Result<bool>.NotFound("Project not found.");
        }

        var alreadyFollowing = await db.Follows.AnyAsync(
            f => f.FollowerUserId == request.RequestingUserId && f.TargetType == FollowTargetType.Project && f.TargetId == project.Id,
            cancellationToken);
        if (alreadyFollowing)
        {
            return Result<bool>.Success(true);
        }

        db.Follows.Add(new Follow
        {
            Id = Guid.NewGuid(),
            FollowerUserId = request.RequestingUserId,
            TargetType = FollowTargetType.Project,
            TargetId = project.Id,
            CreatedAt = DateTimeOffset.UtcNow,
        });
        await db.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}

public record UnfollowProjectCommand(string Slug, Guid RequestingUserId) : IRequest<Result<bool>>;

public class UnfollowProjectCommandHandler(AppDbContext db) : IRequestHandler<UnfollowProjectCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(UnfollowProjectCommand request, CancellationToken cancellationToken)
    {
        var project = await db.Projects.FirstOrDefaultAsync(p => p.Slug == request.Slug, cancellationToken);
        if (project is null)
        {
            return Result<bool>.NotFound("Project not found.");
        }

        var follow = await db.Follows.FirstOrDefaultAsync(
            f => f.FollowerUserId == request.RequestingUserId && f.TargetType == FollowTargetType.Project && f.TargetId == project.Id,
            cancellationToken);
        if (follow is not null)
        {
            db.Follows.Remove(follow);
            await db.SaveChangesAsync(cancellationToken);
        }

        return Result<bool>.Success(true);
    }
}
