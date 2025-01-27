using NotificationService.Application.Models;

namespace NotificationService.Application.Contracts.Notifications.Operations;

public static class GetNotification
{
    public readonly record struct Request(NotificationState? State, int PageSize, string PageToken);

    public readonly record struct Result(IEnumerable<NotificationModel> Items, string? PageToken);
}