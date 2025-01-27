using NotificationService.Application.Models;
using SourceKit.Generators.Builder.Annotations;

namespace NotificationService.Application.Abstractions.Persistence.Queries;

[GenerateBuilder]
public partial record GetNotificationsQuery(NotificationState? State, [RequiredValue] int PageSize, long Cursor);