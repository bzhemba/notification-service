using Itmo.Dev.Platform.Kafka.Consumer;
using NotificationService.Application.Contracts;
using NotificationService.Application.Contracts.Notifications.Operations;
using SubmissionService.Kafka.Contracts;

namespace NotificationService.Presentation.Kafka.ConsumerHandlers;

internal class SubmissionConsumerHandler : IKafkaInboxHandler<DraftNotificationKey, DraftNotificationValue>
{
    private readonly INotificationService _notificationService;

    public SubmissionConsumerHandler(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    public async ValueTask HandleAsync(
        IEnumerable<IKafkaInboxMessage<DraftNotificationKey, DraftNotificationValue>> messages,
        CancellationToken cancellationToken)
    {
        foreach (IKafkaInboxMessage<DraftNotificationKey, DraftNotificationValue> message in messages)
        {
                var request = new CreateNotification.Request(
                    message.Value.ApplicationId,
                    message.Value.UserEmail,
                    message.Value.CreatedAt.ToDateTime());

                await _notificationService.CreateAsync(request, cancellationToken);
        }
    }
}