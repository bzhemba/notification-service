using NotificationService.Application.Abstractions.Commands;
using NotificationService.Application.Abstractions.Persistence.Queries;
using NotificationService.Application.Models;

namespace NotificationService.Application.Abstractions.Persistence.Repositories;

public interface INotificationRepository
{
    IAsyncEnumerable<NotificationModel> GetNotificationsAsync(
        GetNotificationsQuery query,
        CancellationToken cancellationToken);

    Task AddOrUpdateAsync(NotificationModel notification, CancellationToken cancellationToken);

    Task SendAsync(SendNotificationCommand notification, CancellationToken cancellationToken);

    Task<NotificationModel?> GetNotificationByUserAsync(string userEmail, CancellationToken cancellationToken);
}