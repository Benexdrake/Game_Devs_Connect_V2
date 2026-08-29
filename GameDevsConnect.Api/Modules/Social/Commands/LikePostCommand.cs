using GameDevsConnect.Api.Infrastructure.Persistence;
using GameDevsConnect.Api.Modules.Social.Domain;
using GameDevsConnect.Api.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GameDevsConnect.Api.Modules.Social.Commands;

public record LikePostCommand(Guid PostId, Guid RequestingUserId) : IRequest<Result<bool>>;

public class LikePostCommandHandler(AppDbContext db) : IRequestHandler<LikePostCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(LikePostCommand request, CancellationToken cancellationToken)
    {
        var post = await db.Posts.FirstOrDefaultAsync(p => p.Id == request.PostId && !p.IsDeleted, cancellationToken);
        if (post is null)
        {
            return Result<bool>.NotFound("Post not found.");
        }

        var alreadyLiked = await db.Likes.AnyAsync(
            l => l.PostId == request.PostId && l.UserId == request.RequestingUserId, cancellationToken);
        if (!alreadyLiked)
        {
            db.Likes.Add(new Like { UserId = request.RequestingUserId, PostId = request.PostId, CreatedAt = DateTimeOffset.UtcNow });
            await db.SaveChangesAsync(cancellationToken);
        }

        return Result<bool>.Success(true);
    }
}

public record UnlikePostCommand(Guid PostId, Guid RequestingUserId) : IRequest<Result<bool>>;

public class UnlikePostCommandHandler(AppDbContext db) : IRequestHandler<UnlikePostCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(UnlikePostCommand request, CancellationToken cancellationToken)
    {
        var like = await db.Likes.FirstOrDefaultAsync(
            l => l.PostId == request.PostId && l.UserId == request.RequestingUserId, cancellationToken);
        if (like is not null)
        {
            db.Likes.Remove(like);
            await db.SaveChangesAsync(cancellationToken);
        }

        return Result<bool>.Success(true);
    }
}
