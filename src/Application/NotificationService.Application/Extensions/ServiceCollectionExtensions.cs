using Microsoft.Extensions.DependencyInjection;
using NotificationService.Application.Contracts;

namespace NotificationService.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection collection)
    {
        collection.AddScoped<INotificationService, Application.NotificationService>();

        return collection;
    }
}