using MediatR;

namespace GameDevsConnect.Api.Modules.Social.Events;

public record PostCreatedEvent(Guid PostId, Guid ProjectId, Guid AuthorId) : INotification;
