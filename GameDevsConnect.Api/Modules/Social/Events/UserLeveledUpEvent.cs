using MediatR;

namespace GameDevsConnect.Api.Modules.Social.Events;

public record UserLeveledUpEvent(Guid UserId, int NewLevel) : INotification;
