using MediatR;

namespace GameDevsConnect.Api.Modules.Social.Events;

public record QuestCreatedEvent(Guid QuestId, Guid ProjectId, Guid ActorUserId, string QuestTitle) : INotification;
