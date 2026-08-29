using GameDevsConnect.Api.Infrastructure.Persistence;
using GameDevsConnect.Api.Modules.Projects;
using GameDevsConnect.Api.Shared;
using GameDevsConnect.Api.Shared.Storage;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GameDevsConnect.Api.Modules.Contributions.Queries;

public record SubmissionFileContent(Stream Content, string ContentType, string FileName);

public record GetSubmissionFileQuery(Guid SubmissionId, Guid FileId, Guid RequestingUserId) : IRequest<Result<SubmissionFileContent>>;

public class GetSubmissionFileQueryHandler(AppDbContext db, IFileStorage fileStorage)
    : IRequestHandler<GetSubmissionFileQuery, Result<SubmissionFileContent>>
{
    public async Task<Result<SubmissionFileContent>> Handle(GetSubmissionFileQuery request, CancellationToken cancellationToken)
    {
        var file = await db.SubmissionFiles.FirstOrDefaultAsync(
            f => f.Id == request.FileId && f.SubmissionId == request.SubmissionId, cancellationToken);
        if (file is null)
        {
            return Result<SubmissionFileContent>.NotFound("File not found.");
        }

        var submission = await db.QuestSubmissions.FirstAsync(s => s.Id == request.SubmissionId, cancellationToken);
        var quest = await db.Quests.FirstAsync(q => q.Id == submission.QuestId, cancellationToken);

        var isAuthor = submission.UserId == request.RequestingUserId;
        var isProjectMember = await ProjectAccess.GetRoleAsync(db, quest.ProjectId, request.RequestingUserId, cancellationToken) is not null;

        if (!isAuthor && !isProjectMember)
        {
            // 404, not 403 - don't leak the existence of the file.
            return Result<SubmissionFileContent>.NotFound("File not found.");
        }

        var stream = fileStorage.OpenRead(file.StoragePath);
        return Result<SubmissionFileContent>.Success(new SubmissionFileContent(stream, file.ContentType, file.FileName));
    }
}
