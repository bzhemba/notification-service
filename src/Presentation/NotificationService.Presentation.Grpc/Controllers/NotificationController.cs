using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using NotificationService.Application.Contracts;
using NotificationService.Application.Contracts.Notifications.Operations;
using NotificationService.Application.Models;

namespace NotificationService.Presentation.Grpc.Controllers;

public class NotificationController : NotificationService.NotificationServiceBase
{
    private readonly INotificationService _notificationService;

    public NotificationController(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    public override async Task<SendNotificationResponse> SendNotification(
        SendNotificationRequest request,
        ServerCallContext context)
    {
        var applicationCommand = new SendNotification.Request(request.UserEmail);
        SendNotification.Result response
            = await _notificationService.SendNotification(applicationCommand, context.CancellationToken);
        return response switch
        {
            Application.Contracts.Notifications.Operations.SendNotification.Result.NothingToSend => throw new
                RpcException(new Status(StatusCode.NotFound, $"Nothing to send.")),
            Application.Contracts.Notifications.Operations.SendNotification.Result.NotificationAlreadySent => throw new
                RpcException(new Status(StatusCode.AlreadyExists, $"Notification already sent.")),
            Application.Contracts.Notifications.Operations.SendNotification.Result.MissingUser => throw new
                RpcException(new Status(StatusCode.Unknown, $"User is not authorized.")),
            Application.Contracts.Notifications.Operations.SendNotification.Result.Success =>
                new SendNotificationResponse(),
            _ => throw new RpcException(new Status(StatusCode.Cancelled, "Unable to send notification.")),
        };
    }

    public override async Task<GetNotificationsResponse> GetNotifications(
        GetNotificationsRequest request,
        ServerCallContext context)
    {
        var notificationCommand = new GetNotification.Request(
            Convert(request.State),
            request.PageSize,
            request.PageToken);
        GetNotification.Result response
            = await _notificationService.QueryAsync(notificationCommand, context.CancellationToken);
        if (response.Items.Any())
        {
            var notifications = response.Items.Select(Convert).ToList();

            return new GetNotificationsResponse
            {
                Notifications = { notifications },
                NextPageToken = response.PageToken,
            };
        }

        throw new RpcException(new Status(StatusCode.NotFound, "Notifications with this filter not found"));
    }

    private Notification? Convert(NotificationModel notification)
    {
        return new Notification
        {
            ApplicationId = notification.ApplicationId,
            CreatedAt = notification.CreatedAt.ToTimestamp(),
            State = Convert(notification.State),
            UserEmail = notification.UserEmail,
        };
    }

    private Application.Models.NotificationState? Convert(NotificationState? state)
    {
        return state switch
        {
            NotificationState.Pending => Application.Models.NotificationState.Pending,
            NotificationState.Sent => Application.Models.NotificationState.Sent,
            NotificationState.Unspecified => null,
            _ => null,
        };
    }

    private NotificationState Convert(Application.Models.NotificationState? state)
    {
        return state switch
        {
            Application.Models.NotificationState.Pending => NotificationState.Pending,
            Application.Models.NotificationState.Sent => NotificationState.Sent,
            _ => NotificationState.Unspecified,
        };
    }
}