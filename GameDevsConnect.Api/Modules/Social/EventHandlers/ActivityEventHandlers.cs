using System.Text.Json;
using GameDevsConnect.Api.Infrastructure.Persistence;
using GameDevsConnect.Api.Modules.Social.Domain;
using GameDevsConnect.Api.Modules.Social.Events;
using MediatR;

namespace GameDevsConnect.Api.Modules.Social.EventHandlers;

// One ActivityEvent row per instrumented action - the shared source for both
// the project Activity tab and every follower's home feed. A simple
// in-process MediatR notification is enough here; no message bus needed in
// a monolith.
public class QuestCreatedActivityHandler(AppDbContext db) : INotificationHandler<QuestCreatedEvent>
{
    public async Task Handle(QuestCreatedEvent notification, CancellationToken cancellationToken)
    {
        db.ActivityEvents.Add(new ActivityEvent
        {
            Id = Guid.NewGuid(),
            ProjectId = notification.ProjectId,
            QuestId = notification.QuestId,
            ActorUserId = notification.ActorUserId,
            Type = ActivityEventType.QuestCreated,
            Payload = JsonSerializer.Serialize(new { questId = notification.QuestId, questTitle = notification.QuestTitle }),
            CreatedAt = DateTimeOffset.UtcNow,
        });
        await db.SaveChangesAsync(cancellationToken);
    }
}

public class ContributionAcceptedActivityHandler(AppDbContext db) : INotificationHandler<ContributionAcceptedEvent>
{
    public async Task Handle(ContributionAcceptedEvent notification, CancellationToken cancellationToken)
    {
        db.ActivityEvents.Add(new ActivityEvent
        {
            Id = Guid.NewGuid(),
            ProjectId = notification.ProjectId,
            QuestId = notification.QuestId,
            ActorUserId = notification.ContributorUserId,
            Type = ActivityEventType.ContributionAccepted,
            Payload = JsonSerializer.Serialize(new { questId = notification.QuestId, questTitle = notification.QuestTitle }),
            CreatedAt = DateTimeOffset.UtcNow,
        });
        await db.SaveChangesAsync(cancellationToken);
    }
}

public class ProjectMemberJoinedActivityHandler(AppDbContext db) : INotificationHandler<ProjectMemberJoinedEvent>
{
    public async Task Handle(ProjectMemberJoinedEvent notification, CancellationToken cancellationToken)
    {
        db.ActivityEvents.Add(new ActivityEvent
        {
            Id = Guid.NewGuid(),
            ProjectId = notification.ProjectId,
            ActorUserId = notification.UserId,
            Type = ActivityEventType.MemberJoined,
            Payload = null,
            CreatedAt = DateTimeOffset.UtcNow,
        });
        await db.SaveChangesAsync(cancellationToken);
    }
}

public class PostCreatedActivityHandler(AppDbContext db) : INotificationHandler<PostCreatedEvent>
{
    public async Task Handle(PostCreatedEvent notification, CancellationToken cancellationToken)
    {
        db.ActivityEvents.Add(new ActivityEvent
        {
            Id = Guid.NewGuid(),
            ProjectId = notification.ProjectId,
            ActorUserId = notification.AuthorId,
            Type = ActivityEventType.ProjectPosted,
            Payload = JsonSerializer.Serialize(new { postId = notification.PostId }),
            CreatedAt = DateTimeOffset.UtcNow,
        });
        await db.SaveChangesAsync(cancellationToken);
    }
}

public class UserLeveledUpActivityHandler(AppDbContext db) : INotificationHandler<UserLeveledUpEvent>
{
    public async Task Handle(UserLeveledUpEvent notification, CancellationToken cancellationToken)
    {
        db.ActivityEvents.Add(new ActivityEvent
        {
            Id = Guid.NewGuid(),
            ProjectId = null,
            ActorUserId = notification.UserId,
            Type = ActivityEventType.LevelUp,
            Payload = JsonSerializer.Serialize(new { newLevel = notification.NewLevel }),
            CreatedAt = DateTimeOffset.UtcNow,
        });
        await db.SaveChangesAsync(cancellationToken);
    }
}
