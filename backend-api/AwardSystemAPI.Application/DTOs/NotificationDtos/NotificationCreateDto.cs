using System.ComponentModel.DataAnnotations;

namespace AwardSystemAPI.Application.DTOs.NotificationDtos;

public class NotificationCreateDto
{
    [Required]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Description { get; set; } = string.Empty;

    public int UserId { get; set; }
}