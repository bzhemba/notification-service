using Itmo.Dev.Platform.Events;

namespace NotificationService.Application.Contracts.Notifications.Events;

public record NotificationSentEvent(
    long ApplicationId,
    string UserEmail,
    DateTimeOffset CreatedAt) : IEvent;