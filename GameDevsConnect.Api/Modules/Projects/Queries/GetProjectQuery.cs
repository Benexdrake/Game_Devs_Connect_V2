using GameDevsConnect.Api.Infrastructure.Persistence;
using GameDevsConnect.Api.Modules.Projects.Domain;
using GameDevsConnect.Api.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GameDevsConnect.Api.Modules.Projects.Queries;

public record GetProjectQuery(string Slug, Guid? RequestingUserId) : IRequest<Result<ProjectDto>>;

public class GetProjectQueryHandler(AppDbContext db)
    : IRequestHandler<GetProjectQuery, Result<ProjectDto>>
{
    public async Task<Result<ProjectDto>> Handle(GetProjectQuery request, CancellationToken cancellationToken)
    {
        var project = await db.Projects.FirstOrDefaultAsync(p => p.Slug == request.Slug, cancellationToken);
        if (project is null)
        {
            return Result<ProjectDto>.NotFound("Project not found.");
        }

        if (project.Visibility == ProjectVisibility.Private)
        {
            var isMember = request.RequestingUserId is not null &&
                await db.ProjectMembers.AnyAsync(
                    m => m.ProjectId == project.Id && m.UserId == request.RequestingUserId, cancellationToken);

            if (!isMember)
            {
                // 404, not 403 - don't leak the existence of private projects.
                return Result<ProjectDto>.NotFound("Project not found.");
            }
        }

        return Result<ProjectDto>.Success(await ProjectDtoBuilder.BuildAsync(db, project, cancellationToken, request.RequestingUserId));
    }
}
