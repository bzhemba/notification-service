using Itmo.Dev.Platform.Persistence.Abstractions.Commands;
using Itmo.Dev.Platform.Persistence.Abstractions.Connections;
using NotificationService.Application.Abstractions.Commands;
using NotificationService.Application.Abstractions.Persistence.Queries;
using NotificationService.Application.Abstractions.Persistence.Repositories;
using NotificationService.Application.Models;
using System.Data;
using System.Data.Common;
using System.Runtime.CompilerServices;

namespace NotificationService.Infrastructure.Persistence.Repositories;

public class NotificationRepository : INotificationRepository
{
    private readonly IPersistenceConnectionProvider _connectionProvider;

    public NotificationRepository(IPersistenceConnectionProvider connectionProvider)
    {
        _connectionProvider = connectionProvider;
    }

    public async Task AddOrUpdateAsync(NotificationModel notification, CancellationToken cancellationToken)
    {
        const string sql = """
                           INSERT INTO notifications (application_id, user_email, created_at, notification_state)
                           VALUES (:applicationId, :userEmail, :createdAt, :notificationState)
                           ON CONFLICT (application_id, user_email)
                           DO UPDATE SET created_at = EXCLUDED.created_at;
                           """;

        await using IPersistenceConnection connection = await _connectionProvider.GetConnectionAsync(cancellationToken);

        await using IPersistenceCommand command = connection.CreateCommand(sql)
            .AddParameter("applicationId", notification.ApplicationId)
            .AddParameter("userEmail", notification.UserEmail)
            .AddParameter("notificationState", NotificationState.Pending)
            .AddParameter("createdAt", DateTimeOffset.UtcNow);

        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    public async Task SendAsync(
        SendNotificationCommand notification,
        CancellationToken cancellationToken)
    {
        const string sql = """
                           update notifications set notification_state = :newState
                           where user_email = :userEmail;
                           """;

        await using IPersistenceConnection connection = await _connectionProvider.GetConnectionAsync(cancellationToken);
        await using IPersistenceCommand command = connection.CreateCommand(sql)
            .AddParameter("userEmail", notification.UserEmail)
            .AddParameter("newState", NotificationState.Sent);

        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    public async IAsyncEnumerable<NotificationModel> GetNotificationsAsync(
        GetNotificationsQuery query,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        const string sql = """
                           select application_id,
                                  user_email,
                                  notification_state,
                                  created_at
                           from notifications
                           where 
                           notification_state = :state
                           and application_id > :cursor
                           limit :page_size;
                           """;

        await using IPersistenceConnection connection = await _connectionProvider.GetConnectionAsync(cancellationToken);

        await using IPersistenceCommand command = connection.CreateCommand(sql)
            .AddParameter("state", query.State)
            .AddParameter("cursor", query.Cursor)
            .AddParameter("page_size", query.PageSize);
        await using DbDataReader reader = await command.ExecuteReaderAsync(cancellationToken);

        while (await reader.ReadAsync(cancellationToken))
        {
            yield return new NotificationModel(
                ApplicationId: reader.GetInt64("application_id"),
                UserEmail: reader.GetString("user_email"),
                CreatedAt: reader.GetFieldValue<DateTimeOffset>("created_at"),
                State: reader.GetFieldValue<NotificationState>("notification_state"));
        }
    }

    public async Task<NotificationModel?> GetNotificationByUserAsync(string userEmail, CancellationToken cancellationToken)
    {
        const string sql = """
                           select application_id,
                                  user_email,
                                  created_at,
                                  notification_state
                           from notifications
                           where 
                           user_email = :userEmail
                           """;

        await using IPersistenceConnection connection = await _connectionProvider.GetConnectionAsync(cancellationToken);

        await using IPersistenceCommand command = connection.CreateCommand(sql)
            .AddParameter("userEmail", userEmail);
        await using DbDataReader reader = await command.ExecuteReaderAsync(cancellationToken);
        await reader.ReadAsync(cancellationToken);

        return reader.HasRows
            ? new NotificationModel(
                ApplicationId: reader.GetInt64("application_id"),
                UserEmail: reader.GetString("user_email"),
                CreatedAt: reader.GetFieldValue<DateTimeOffset>("created_at"),
                State: reader.GetFieldValue<NotificationState>("notification_state"))
            : null;
    }
}