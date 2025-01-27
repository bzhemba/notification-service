namespace NotificationService.Application.Models;

public sealed record NotificationModel(
    long ApplicationId,
    string UserEmail,
    DateTimeOffset CreatedAt,
    NotificationState State);