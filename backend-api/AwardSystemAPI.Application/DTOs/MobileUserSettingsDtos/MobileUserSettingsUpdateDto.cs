using System.ComponentModel.DataAnnotations;

namespace AwardSystemAPI.Application.DTOs.MobileUserSettingsDtos;

public class MobileUserSettingsUpdateDto
{
    [Required]
    public bool PushNotifications { get; set; }
        
    [Required]
    public bool AiFunctionality { get; set; }
}