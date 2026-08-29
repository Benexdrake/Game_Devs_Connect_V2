using MediatR;

namespace GameDevsConnect.Api.Modules.Social.Events;

public record UserFollowedEvent(Guid TargetUserId, Guid FollowerUserId) : INotification;
