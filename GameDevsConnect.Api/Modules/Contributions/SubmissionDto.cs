using GameDevsConnect.Api.Infrastructure.Persistence;
using GameDevsConnect.Api.Modules.Contributions.Domain;
using Microsoft.EntityFrameworkCore;

namespace GameDevsConnect.Api.Modules.Contributions;

public record SubmissionFileDto(Guid Id, string FileName, string ContentType, long SizeBytes, DateTimeOffset UploadedAt);

public record SubmissionLinkDto(Guid Id, string Url, string? Label);

public record SubmissionDto(
    Guid Id,
    Guid QuestId,
    Guid UserId,
    string Username,
    string Description,
    SubmissionStatus Status,
    DateTimeOffset SubmittedAt,
    DateTimeOffset? ReviewedAt,
    Guid? ReviewerId,
    string? ReviewComment,
    IReadOnlyList<SubmissionFileDto> Files,
    IReadOnlyList<SubmissionLinkDto> Links);

internal static class SubmissionDtoBuilder
{
    public static async Task<SubmissionDto> BuildAsync(AppDbContext db, QuestSubmission submission, CancellationToken ct)
    {
        var author = await db.Users.FirstAsync(u => u.Id == submission.UserId, ct);

        var files = await db.SubmissionFiles
            .Where(f => f.SubmissionId == submission.Id)
            .Select(f => new SubmissionFileDto(f.Id, f.FileName, f.ContentType, f.SizeBytes, f.UploadedAt))
            .ToListAsync(ct);

        var links = await db.SubmissionLinks
            .Where(l => l.SubmissionId == submission.Id)
            .Select(l => new SubmissionLinkDto(l.Id, l.Url, l.Label))
            .ToListAsync(ct);

        return new SubmissionDto(
            submission.Id,
            submission.QuestId,
            submission.UserId,
            author.Username,
            submission.Description,
            submission.Status,
            submission.SubmittedAt,
            submission.ReviewedAt,
            submission.ReviewerId,
            submission.ReviewComment,
            files,
            links);
    }
}
