namespace AwardSystemAPI.Application.DTOs.MobileUserSettingsDtos;

public class MobileUserSettingsResponseDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public bool PushNotifications { get; set; }
    public bool AiFunctionality { get; set; }
}