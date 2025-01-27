using SourceKit.Generators.Builder.Annotations;

namespace NotificationService.Application.Abstractions.Commands;

[GenerateBuilder]
public partial record SendNotificationCommand(string UserEmail);