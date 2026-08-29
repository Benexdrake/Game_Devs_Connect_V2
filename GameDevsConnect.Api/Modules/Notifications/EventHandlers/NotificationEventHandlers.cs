using GameDevsConnect.Api.Infrastructure.Persistence;
using GameDevsConnect.Api.Modules.Notifications.Domain;
using GameDevsConnect.Api.Modules.Social.Domain;
using GameDevsConnect.Api.Modules.Social.Events;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GameDevsConnect.Api.Modules.Notifications.EventHandlers;

// Only the three notification triggers called out in the Phase 5 spec:
// submission author on review result, followers on a new quest in a
// followed project, and a user on a new follower. Everything else (member
// joined, posts, level-ups) shows up via the ActivityEvent feed/activity
// tab instead of a push notification.
public class SubmissionReviewedNotificationHandler(AppDbContext db) : INotificationHandler<SubmissionReviewedEvent>
{
    public async Task Handle(SubmissionReviewedEvent notification, CancellationToken cancellationToken)
    {
        db.Notifications.Add(new Notification
        {
            Id = Guid.NewGuid(),
            UserId = notification.AuthorUserId,
            Type = NotificationType.SubmissionReviewed,
            Message = $"Your submission for \"{notification.QuestTitle}\" was reviewed: {notification.Decision}.",
            ActivityEventId = null,
            IsRead = false,
            CreatedAt = DateTimeOffset.UtcNow,
        });
        await db.SaveChangesAsync(cancellationToken);
    }
}

public class NewQuestFollowerNotificationHandler(AppDbContext db) : INotificationHandler<QuestCreatedEvent>
{
    public async Task Handle(QuestCreatedEvent notification, CancellationToken cancellationToken)
    {
        var followerIds = await db.Follows
            .Where(f => f.TargetType == FollowTargetType.Project && f.TargetId == notification.ProjectId
                && f.FollowerUserId != notification.ActorUserId)
            .Select(f => f.FollowerUserId)
            .ToListAsync(cancellationToken);

        if (followerIds.Count == 0)
        {
            return;
        }

        var now = DateTimeOffset.UtcNow;
        foreach (var followerId in followerIds)
        {
            db.Notifications.Add(new Notification
            {
                Id = Guid.NewGuid(),
                UserId = followerId,
                Type = NotificationType.NewQuestInFollowedProject,
                Message = $"New quest \"{notification.QuestTitle}\" in a project you follow.",
                ActivityEventId = null,
                IsRead = false,
                CreatedAt = now,
            });
        }

        await db.SaveChangesAsync(cancellationToken);
    }
}

public class UserFollowedNotificationHandler(AppDbContext db) : INotificationHandler<UserFollowedEvent>
{
    public async Task Handle(UserFollowedEvent notification, CancellationToken cancellationToken)
    {
        var follower = await db.Users.FirstAsync(u => u.Id == notification.FollowerUserId, cancellationToken);

        db.Notifications.Add(new Notification
        {
            Id = Guid.NewGuid(),
            UserId = notification.TargetUserId,
            Type = NotificationType.NewFollower,
            Message = $"{follower.Username} started following you.",
            ActivityEventId = null,
            IsRead = false,
            CreatedAt = DateTimeOffset.UtcNow,
        });
        await db.SaveChangesAsync(cancellationToken);
    }
}
