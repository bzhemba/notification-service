using Itmo.Dev.Platform.Events;
using Itmo.Dev.Platform.Persistence.Abstractions.Transactions;
using NotificationService.Application.Abstractions.Commands;
using NotificationService.Application.Abstractions.Persistence;
using NotificationService.Application.Abstractions.Persistence.Queries;
using NotificationService.Application.Contracts;
using NotificationService.Application.Contracts.Notifications.Events;
using NotificationService.Application.Contracts.Notifications.Operations;
using NotificationService.Application.Models;
using System.Data;

namespace NotificationService.Application.Application;

internal class NotificationService : INotificationService
{
    private readonly IPersistenceContext _context;
    private readonly IPersistenceTransactionProvider _transactionProvider;
    private readonly IEventPublisher _eventPublisher;

    public NotificationService(
        IPersistenceContext context,
        IEventPublisher eventPublisher,
        IPersistenceTransactionProvider transactionProvider)
    {
        _context = context;
        _eventPublisher = eventPublisher;
        _transactionProvider = transactionProvider;
    }

    public async Task CreateAsync(
        CreateNotification.Request request,
        CancellationToken cancellationToken)
    {
        var command = new NotificationModel(
            request.ApplicationId,
            request.UserEmail,
            DateTimeOffset.UtcNow,
            NotificationState.Pending);

        await _context.Notifications.AddOrUpdateAsync(command, cancellationToken);
    }

    public async Task<SendNotification.Result> SendNotification(
        SendNotification.Request request,
        CancellationToken cancellationToken)
    {
        var command = new SendNotificationCommand(request.UserEmail);
        NotificationModel? notification =
            await _context.Notifications.GetNotificationByUserAsync(request.UserEmail, cancellationToken);
        if (notification == null)
        {
            return new SendNotification.Result.NothingToSend();
        }

        if (notification.State == NotificationState.Sent)
        {
            return new SendNotification.Result.NotificationAlreadySent();
        }

        var evt = new NotificationSentEvent(notification.ApplicationId, notification.UserEmail, notification.CreatedAt);

        await using IPersistenceTransaction transaction = await _transactionProvider.BeginTransactionAsync(
            IsolationLevel.ReadCommitted,
            cancellationToken);

        await _context.Notifications.SendAsync(command, cancellationToken);
        await _eventPublisher.PublishAsync(evt, cancellationToken);

        await transaction.CommitAsync(cancellationToken);

        return new SendNotification.Result.Success();
    }

    public async Task<GetNotification.Result> QueryAsync(
        GetNotification.Request request,
        CancellationToken cancellationToken)
    {
        int cursor = int.TryParse(request.PageToken, out int value) ? value : 0;
        var query = new GetNotificationsQuery(request.State, request.PageSize, cursor);
        NotificationModel[] notifications = await _context.Notifications.GetNotificationsAsync(query, cancellationToken)
            .ToArrayAsync(cancellationToken);
        string? pageToken = notifications.Length == request.PageSize ? notifications[^1].ApplicationId.ToString() : null;

        IEnumerable<NotificationModel> dto = notifications
            .Select(x =>
                new NotificationModel(
                    x.ApplicationId,
                    x.UserEmail,
                    x.CreatedAt,
                    x.State));

        return new GetNotification.Result(dto, pageToken);
    }
}