using Microsoft.Extensions.DependencyInjection;

namespace NotificationService.Presentation.Grpc.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPresentationGrpc(this IServiceCollection collection)
    {
        collection.AddGrpc(grpc => grpc.Interceptors.Add<ExceptionInterceptor>());
        collection.AddGrpcReflection();

        return collection;
    }
}