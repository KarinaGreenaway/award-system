using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AwardSystemAPI.Domain.Entities;

[Table("mobile_user_settings")]
public class MobileUserSettings
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int UserId { get; set; }

    public bool PushNotifications { get; set; } = true;

    public bool AiFunctionality { get; set; } = true;

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Required]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}