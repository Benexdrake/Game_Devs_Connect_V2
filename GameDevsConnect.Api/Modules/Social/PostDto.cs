using GameDevsConnect.Api.Infrastructure.Persistence;
using GameDevsConnect.Api.Modules.Social.Domain;
using Microsoft.EntityFrameworkCore;

namespace GameDevsConnect.Api.Modules.Social;

public record PostAttachmentDto(Guid Id, string FileName, string ContentType);

public record CommentDto(Guid Id, Guid AuthorId, string AuthorUsername, string Body, DateTimeOffset CreatedAt);

public record PostDto(
    Guid Id,
    Guid ProjectId,
    Guid AuthorId,
    string AuthorUsername,
    string Body,
    DateTimeOffset CreatedAt,
    IReadOnlyList<PostAttachmentDto> Attachments,
    IReadOnlyList<CommentDto> Comments,
    int LikeCount,
    bool LikedByMe);

internal static class PostDtoBuilder
{
    public static async Task<PostDto> BuildAsync(AppDbContext db, Post post, Guid? requestingUserId, CancellationToken ct)
    {
        var author = await db.Users.FirstAsync(u => u.Id == post.AuthorId, ct);

        var attachments = await db.PostAttachments
            .Where(a => a.PostId == post.Id)
            .Select(a => new PostAttachmentDto(a.Id, a.FileName, a.ContentType))
            .ToListAsync(ct);

        var comments = await db.Comments
            .Where(c => c.PostId == post.Id && !c.IsDeleted)
            .OrderBy(c => c.CreatedAt)
            .Join(db.Users, c => c.AuthorId, u => u.Id,
                (c, u) => new CommentDto(c.Id, u.Id, u.Username, c.Body, c.CreatedAt))
            .ToListAsync(ct);

        var likeCount = await db.Likes.CountAsync(l => l.PostId == post.Id, ct);
        var likedByMe = requestingUserId is not null &&
            await db.Likes.AnyAsync(l => l.PostId == post.Id && l.UserId == requestingUserId, ct);

        return new PostDto(
            post.Id,
            post.ProjectId,
            post.AuthorId,
            author.Username,
            post.Body,
            post.CreatedAt,
            attachments,
            comments,
            likeCount,
            likedByMe);
    }
}
