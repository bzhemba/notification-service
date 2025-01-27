using Itmo.Dev.Platform.Persistence.Postgres.Plugins;
using NotificationService.Application.Models;
using Npgsql;

namespace NotificationService.Infrastructure.Persistence.Plugins;

public class MappingPlugin : IPostgresDataSourcePlugin
{
    public void Configure(NpgsqlDataSourceBuilder dataSource)
    {
        dataSource.MapEnum<NotificationState>("notification_state");
    }
}