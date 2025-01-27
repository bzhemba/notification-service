using NotificationService.Application.Abstractions.Persistence.Repositories;

namespace NotificationService.Application.Abstractions.Persistence;

public interface IPersistenceContext
{
    INotificationRepository Notifications { get; }
}