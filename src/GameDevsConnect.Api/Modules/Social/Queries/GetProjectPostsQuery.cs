using GameDevsConnect.Api.Infrastructure.Persistence;
using GameDevsConnect.Api.Modules.Quests;
using GameDevsConnect.Api.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GameDevsConnect.Api.Modules.Social.Queries;

public record GetProjectPostsQuery(string ProjectSlug, Guid? RequestingUserId) : IRequest<Result<IReadOnlyList<PostDto>>>;

public class GetProjectPostsQueryHandler(AppDbContext db) : IRequestHandler<GetProjectPostsQuery, Result<IReadOnlyList<PostDto>>>
{
    public async Task<Result<IReadOnlyList<PostDto>>> Handle(GetProjectPostsQuery request, CancellationToken cancellationToken)
    {
        var project = await db.Projects.FirstOrDefaultAsync(p => p.Slug == request.ProjectSlug, cancellationToken);
        if (project is null)
        {
            return Result<IReadOnlyList<PostDto>>.NotFound("Project not found.");
        }

        if (!await QuestAccess.CanViewProjectQuestsAsync(db, project, request.RequestingUserId, cancellationToken))
        {
            return Result<IReadOnlyList<PostDto>>.NotFound("Project not found.");
        }

        var posts = await db.Posts
            .Where(p => p.ProjectId == project.Id && !p.IsDeleted)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync(cancellationToken);

        var dtos = new List<PostDto>(posts.Count);
        foreach (var post in posts)
        {
            dtos.Add(await PostDtoBuilder.BuildAsync(db, post, request.RequestingUserId, cancellationToken));
        }

        return Result<IReadOnlyList<PostDto>>.Success(dtos);
    }
}
