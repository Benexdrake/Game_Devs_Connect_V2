using MediatR;

namespace GameDevsConnect.Api.Modules.Social.Events;

public record ContributionAcceptedEvent(
    Guid ContributionId,
    Guid ProjectId,
    Guid QuestId,
    Guid ContributorUserId,
    string QuestTitle) : INotification;
