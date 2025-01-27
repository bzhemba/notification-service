using NotificationService.Application.Abstractions.Persistence;
using NotificationService.Application.Abstractions.Persistence.Repositories;

namespace NotificationService.Infrastructure.Persistence;

public class PersistenceContext : IPersistenceContext
{
    public PersistenceContext(INotificationRepository notifications)
    {
        Notifications = notifications;
    }

    public INotificationRepository Notifications { get; }
}