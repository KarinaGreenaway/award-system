using System.ComponentModel.DataAnnotations;
using AwardSystemAPI.Domain.Enums;

namespace AwardSystemAPI.Application.DTOs;

public class AnnouncementCreateDto
{
    [Required]
    [MaxLength(255)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [MaxLength(2000)]
    public string Description { get; set; } = string.Empty;

    [MaxLength(255)]
    public string? ImageUrl { get; set; }

    public bool IsPushNotification { get; set; }

    public DateTime? ScheduledTime { get; set; }
    
    public required string Status { get; set; }

    [Required]
    public TargetAudience Audience { get; set; }
}
    
public class AnnouncementUpdateDto
{
    [Required]
    [MaxLength(255)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [MaxLength(2000)]
    public string Description { get; set; } = string.Empty;

    [MaxLength(255)]
    public string? ImageUrl { get; set; }

    public bool IsPushNotification { get; set; }

    public DateTime? ScheduledTime { get; set; }
    
    public required string Status { get; set; }

    [Required]
    public TargetAudience Audience { get; set; }
}

public class AnnouncementResponseDto
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string? ImageUrl { get; set; }

    public bool IsPushNotification { get; set; }

    public DateTime? ScheduledTime { get; set; }
    
    public required string Status { get; set; }

    public TargetAudience Audience { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}