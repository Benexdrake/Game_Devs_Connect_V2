using GameDevsConnect.Api.Infrastructure.Persistence;
using GameDevsConnect.Api.Modules.Contributions.Domain;
using GameDevsConnect.Api.Shared;
using GameDevsConnect.Api.Shared.Storage;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GameDevsConnect.Api.Modules.Contributions.Commands;

public record UploadSubmissionFilesCommand(
    Guid SubmissionId,
    Guid RequestingUserId,
    IReadOnlyList<UploadedFileInput> Files) : IRequest<Result<IReadOnlyList<SubmissionFileDto>>>;

public class UploadSubmissionFilesCommandHandler(AppDbContext db, IFileStorage fileStorage)
    : IRequestHandler<UploadSubmissionFilesCommand, Result<IReadOnlyList<SubmissionFileDto>>>
{
    public async Task<Result<IReadOnlyList<SubmissionFileDto>>> Handle(UploadSubmissionFilesCommand request, CancellationToken cancellationToken)
    {
        var submission = await db.QuestSubmissions.FirstOrDefaultAsync(s => s.Id == request.SubmissionId, cancellationToken);
        if (submission is null)
        {
            return Result<IReadOnlyList<SubmissionFileDto>>.NotFound("Submission not found.");
        }

        if (submission.UserId != request.RequestingUserId)
        {
            return Result<IReadOnlyList<SubmissionFileDto>>.Forbidden("Only the submission author can upload files.");
        }

        if (submission.Status is not (SubmissionStatus.PendingReview or SubmissionStatus.ChangesRequested))
        {
            return Result<IReadOnlyList<SubmissionFileDto>>.Conflict("Files can only be added while the submission is under review or changes have been requested.");
        }

        var now = DateTimeOffset.UtcNow;
        foreach (var file in request.Files)
        {
            var safeName = Path.GetFileName(file.FileName);
            var fileId = Guid.NewGuid();
            var storagePath = $"submissions/{submission.Id}/{fileId}-{safeName}";

            await fileStorage.SaveAsync(storagePath, file.Content, cancellationToken);

            db.SubmissionFiles.Add(new SubmissionFile
            {
                Id = fileId,
                SubmissionId = submission.Id,
                FileName = safeName,
                ContentType = file.ContentType,
                SizeBytes = file.SizeBytes,
                StoragePath = storagePath,
                UploadedAt = now,
            });
        }

        await db.SaveChangesAsync(cancellationToken);

        var files = await db.SubmissionFiles
            .Where(f => f.SubmissionId == submission.Id)
            .Select(f => new SubmissionFileDto(f.Id, f.FileName, f.ContentType, f.SizeBytes, f.UploadedAt))
            .ToListAsync(cancellationToken);

        return Result<IReadOnlyList<SubmissionFileDto>>.Success(files);
    }
}
