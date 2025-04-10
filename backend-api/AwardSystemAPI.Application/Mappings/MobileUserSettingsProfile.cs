using AutoMapper;
using AwardSystemAPI.Application.DTOs.MobileUserSettingsDtos;
using AwardSystemAPI.Domain.Entities;

namespace AwardSystemAPI.Application.Mappings;

public class MobileUserSettingsProfile: Profile
{
    public MobileUserSettingsProfile()
    {
        CreateMap<MobileUserSettings, MobileUserSettingsResponseDto>();
        CreateMap<MobileUserSettingsUpdateDto, MobileUserSettings>();
    }
    
}