using HttpGateway.Services;
using Microsoft.AspNetCore.Mvc;
using NotificationService;

namespace HttpGateway.Controllers;

[ApiController]
[Route("[controller]")]
public class NotificationsController : ControllerBase
{
    private readonly NotificationGrpcService _service;

    public NotificationsController(NotificationGrpcService notificationService)
    {
        _service = notificationService;
    }

    [HttpPost]
    [Route("/notification/send")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> SendNotification(
        [FromBody] SendNotificationRequest createApplication,
        CancellationToken cancellationToken)
    {
        await _service.SendNotificationAsync(createApplication, cancellationToken);
        return Ok();
    }

    [HttpGet]
    [Route("/notifications")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> GetNotifications(
        NotificationState? state,
        int pageSize,
        string? pageToken,
        CancellationToken cancellationToken)
    {
        GetNotificationsResponse response =
            await _service.GetNotificationsAsync(state, pageSize, pageToken, cancellationToken);
        return Ok(response);
    }
}