using MediatR;

namespace GameDevsConnect.Api.Modules.Social.Events;

public record SubmissionReviewedEvent(
    Guid SubmissionId,
    Guid AuthorUserId,
    Guid QuestId,
    string QuestTitle,
    string Decision) : INotification;
