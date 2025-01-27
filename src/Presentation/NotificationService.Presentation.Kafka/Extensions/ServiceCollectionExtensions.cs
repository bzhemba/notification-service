using Itmo.Dev.Platform.Kafka.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NotificationService.Kafka.Contracts;
using NotificationService.Presentation.Kafka.ConsumerHandlers;
using SubmissionService.Kafka.Contracts;

namespace NotificationService.Presentation.Kafka.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPresentationKafka(
        this IServiceCollection collection,
        IConfiguration configuration)
    {
        const string consumerKey = "Presentation:Kafka:Consumers";
        const string producerKey = "Presentation:Kafka:Producers";

        collection.AddPlatformKafka(kafka => kafka
            .ConfigureOptions(configuration.GetSection("Presentation:Kafka"))
            .AddConsumer(b => b
                .WithKey<DraftNotificationKey>()
                .WithValue<DraftNotificationValue>()
                .WithConfiguration(configuration.GetSection($"{consumerKey}:DraftNotification"))
                .DeserializeKeyWithProto()
                .DeserializeValueWithProto()
                .HandleInboxWith<SubmissionConsumerHandler>())
            .AddProducer(producer => producer
                .WithKey<NotificationCreationKey>()
                .WithValue<NotificationCreationValue>()
                .WithConfiguration(configuration.GetSection($"{producerKey}:UserNotification"))
                .SerializeKeyWithProto()
                .SerializeValueWithProto()
                .WithOutbox()));

        return collection;
    }
}