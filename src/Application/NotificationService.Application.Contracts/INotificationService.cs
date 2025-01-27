using NotificationService.Application.Contracts.Notifications.Operations;

namespace NotificationService.Application.Contracts;

public interface INotificationService
{
    Task CreateAsync(
        CreateNotification.Request request,
        CancellationToken cancellationToken);

    Task<SendNotification.Result> SendNotification(SendNotification.Request request, CancellationToken cancellationToken);

    Task<GetNotification.Result> QueryAsync(GetNotification.Request request, CancellationToken cancellationToken);
}