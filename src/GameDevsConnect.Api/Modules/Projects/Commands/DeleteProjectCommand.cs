using GameDevsConnect.Api.Infrastructure.Persistence;
using GameDevsConnect.Api.Modules.Projects.Domain;
using GameDevsConnect.Api.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GameDevsConnect.Api.Modules.Projects.Commands;

public record DeleteProjectCommand(string Slug, Guid RequestingUserId) : IRequest<Result<bool>>;

public class DeleteProjectCommandHandler(AppDbContext db)
    : IRequestHandler<DeleteProjectCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(DeleteProjectCommand request, CancellationToken cancellationToken)
    {
        var project = await db.Projects.FirstOrDefaultAsync(p => p.Slug == request.Slug, cancellationToken);
        if (project is null)
        {
            return Result<bool>.NotFound("Project not found.");
        }

        var role = await ProjectAccess.GetRoleAsync(db, project.Id, request.RequestingUserId, cancellationToken);
        if (role is not ProjectRole.Owner)
        {
            return Result<bool>.Forbidden("Only the owner can delete this project.");
        }

        db.Projects.Remove(project);
        await db.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}
