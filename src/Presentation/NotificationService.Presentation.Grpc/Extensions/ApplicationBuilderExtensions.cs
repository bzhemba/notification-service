using Microsoft.AspNetCore.Builder;
using NotificationService.Presentation.Grpc.Controllers;

namespace NotificationService.Presentation.Grpc.Extensions;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UsePresentationGrpc(this IApplicationBuilder builder)
    {
        builder.UseEndpoints(routeBuilder =>
        {
            routeBuilder.MapGrpcService<NotificationController>();
            routeBuilder.MapGrpcReflectionService();
        });

        return builder;
    }
}