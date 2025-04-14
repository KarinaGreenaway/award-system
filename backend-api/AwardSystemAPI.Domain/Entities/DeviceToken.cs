using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AwardSystemAPI.Domain.Entities;

[Table("deviceToken")]
public class DeviceToken
{
    [Key] public int Id { get; set; }
    [Required] public int UserId { get; set; } // who owns this device
    [Required, MaxLength(255)]
    public string Token { get; set; } = string.Empty; // e.g. FCM token
    public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;
}
