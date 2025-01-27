using SourceKit.Generators.Builder.Annotations;

namespace NotificationService.Application.Abstractions.Commands;

[GenerateBuilder]
public partial record CreateNotificationCommand(
    long ApplicationId,
    long UserId,
    DateTime CreatedAt);