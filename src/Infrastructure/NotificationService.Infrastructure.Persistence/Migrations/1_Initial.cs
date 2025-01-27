using FluentMigrator;
using Itmo.Dev.Platform.Persistence.Postgres.Migrations;

namespace NotificationService.Infrastructure.Persistence.Migrations;

[Migration(1, "initial")]
#pragma warning disable SA1649
public class Initial : SqlMigration
#pragma warning restore SA1649
{
    protected override string GetUpSql(IServiceProvider serviceProvider) =>
        """
        create type notification_state as enum
        (
            'sent',
            'pending'
        );

        CREATE TABLE notifications (
            application_id bigint NOT NULL,
            user_email TEXT NOT NULL,
            created_at timestamp with time zone,
            notification_state notification_state not null,
            PRIMARY KEY (application_id, user_email)
        );
        """;

    protected override string GetDownSql(IServiceProvider serviceProvider) =>
        """
        drop table notifications;
        drop type notification_state;
        """;
}