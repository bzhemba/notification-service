using NotificationService;

namespace HttpGateway.Services;

public class NotificationGrpcService
{
    private readonly NotificationService.NotificationService.NotificationServiceClient _notificationServiceClient;

    public NotificationGrpcService(
        NotificationService.NotificationService.NotificationServiceClient notificationServiceClient)
    {
        _notificationServiceClient = notificationServiceClient;
    }

    public async Task<SendNotificationResponse> SendNotificationAsync(
        SendNotificationRequest request,
        CancellationToken cancellationToken)
    {
        var grpcRequest = new SendNotificationRequest()
        {
            UserEmail = request.UserEmail,
        };

        return await _notificationServiceClient.SendNotificationAsync(
            grpcRequest,
            cancellationToken: cancellationToken);
    }

    public async Task<GetNotificationsResponse> GetNotificationsAsync(
        NotificationState? state,
        int pageSize,
        string? pageToken,
        CancellationToken cancellationToken)
    {
        var grpcRequest = new GetNotificationsRequest()
        {
            State = state ?? NotificationState.Unspecified,
            PageSize = pageSize,
            PageToken = pageToken,
        };

        return await _notificationServiceClient.GetNotificationsAsync(
            grpcRequest,
            cancellationToken: cancellationToken);
    }
}