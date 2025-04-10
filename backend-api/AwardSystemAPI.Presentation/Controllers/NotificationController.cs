using AwardSystemAPI.Application.Services;
using AwardSystemAPI.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AwardSystemAPI.Application.DTOs.NotificationDtos;

namespace AwardSystemAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class NotificationController : ControllerBase
{
    private readonly INotificationService _notificationService;
    private readonly ILogger<NotificationController> _logger;

    public NotificationController(INotificationService notificationService, ILogger<NotificationController> logger)
    {
        _notificationService = notificationService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<NotificationResponseDto>>> GetMyNotifications()
    {
        var userId = User.GetUserId();
        if (userId == null)
        {
            _logger.LogWarning("User ID claim not found.");
            return Unauthorized(new { Error = "User ID is missing from the token." });
        }

        var response = await _notificationService.GetNotificationsAsync(userId.Value);
        return response.Match<ActionResult>(
            onSuccess: result => Ok(result),
            onError: error =>
            {
                _logger.LogError("Failed to retrieve notifications for user {UserId}. Error: {Error}", userId, error);
                return BadRequest(new { Error = error });
            }
        );
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<NotificationResponseDto>> GetById(int id)
    {
        if (id <= 0)
        {
            _logger.LogWarning("Invalid notification ID {Id} provided.", id);
            return BadRequest(new { Error = "Invalid notification ID provided." });
        }

        var response = await _notificationService.GetNotificationByIdAsync(id);
        return response.Match<ActionResult>(
            onSuccess: result => Ok(result),
            onError: error =>
            {
                _logger.LogError("Failed to retrieve notification with ID {Id}. Error: {Error}", id, error);
                return error.Contains("not found")
                    ? NotFound(new { Error = error })
                    : BadRequest(new { Error = error });
            }
        );
    }

    [HttpPost("{id:int}/read")]
    public async Task<IActionResult> MarkAsRead(int id)
    {
        if (id <= 0)
        {
            _logger.LogWarning("Invalid notification ID {Id} provided.", id);
            return BadRequest(new { Error = "Invalid notification ID provided." });
        }

        var response = await _notificationService.MarkAsReadAsync(id);
        return response.Match<IActionResult>(
            onSuccess: success =>
            {
                _logger.LogInformation("Notification with ID {Id} marked as read.", id);
                return Ok();
            },
            onError: error =>
            {
                _logger.LogError("Failed to mark notification with ID {Id} as read. Error: {Error}", id, error);
                return error.Contains("not found")
                    ? NotFound(new { Error = error })
                    : BadRequest(new { Error = error });
            }
        );
    }
    
    [HttpPost]
    public async Task<ActionResult<NotificationResponseDto>> Create([FromBody] NotificationCreateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var userId = User.GetUserId();
        if (userId == null)
        {
            _logger.LogWarning("User ID claim not found.");
            return Unauthorized(new { Error = "User ID is missing from the token." });
        }
        dto.UserId = userId.Value;

        var response = await _notificationService.CreateNotificationAsync(dto);
        return response.Match<ActionResult>(
            onSuccess: created =>
            {
                _logger.LogInformation("Created notification with ID {Id}.", created.Id);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            },
            onError: error =>
            {
                _logger.LogError("Failed to create notification. Error: {Error}", error);
                return BadRequest(new { Error = error });
            }
        );
    }
}