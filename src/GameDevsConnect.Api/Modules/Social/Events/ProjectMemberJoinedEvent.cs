using MediatR;

namespace GameDevsConnect.Api.Modules.Social.Events;

public record ProjectMemberJoinedEvent(Guid ProjectId, Guid UserId) : INotification;
