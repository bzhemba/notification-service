using Google.Protobuf.WellKnownTypes;
using Itmo.Dev.Platform.Events;
using Itmo.Dev.Platform.Kafka.Extensions;
using Itmo.Dev.Platform.Kafka.Producer;
using NotificationService.Kafka.Contracts;

namespace NotificationService.Presentation.Kafka.ProducerHandlers;

internal class NotificationSentEventHandler : IEventHandler<Application.Contracts.Notifications.Events.NotificationSentEvent>
{
    private readonly IKafkaMessageProducer<NotificationCreationKey, NotificationCreationValue> _producer;

    public NotificationSentEventHandler(IKafkaMessageProducer<NotificationCreationKey, NotificationCreationValue> producer)
    {
        _producer = producer;
    }

    public async ValueTask HandleAsync(Application.Contracts.Notifications.Events.NotificationSentEvent evt, CancellationToken cancellationToken)
    {
        var key = new NotificationCreationKey { UserEmail = evt.UserEmail };

        var value = new NotificationCreationValue
        {
            ApplicationId = evt.ApplicationId,
            CreatedAt = evt.CreatedAt.ToTimestamp(),
        };

        var message = new KafkaProducerMessage<NotificationCreationKey, NotificationCreationValue>(key, value);
        await _producer.ProduceAsync(message, cancellationToken);
    }
}