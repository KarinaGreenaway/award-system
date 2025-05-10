using AutoMapper;
using AwardSystemAPI.Application.DTOs.AwardCategoryDtos;
using AwardSystemAPI.Domain.Entities;

namespace AwardSystemAPI.Application.Mappings;

public class AwardCategoryProfile: Profile
{
    public AwardCategoryProfile()
    {
        CreateMap<AwardCategory, AwardCategoryResponseDto>()
            .ForMember(dest => dest.SponsorName,
                opt => opt.MapFrom(src => src.Sponsor.DisplayName));
        CreateMap<AwardCategoryCreateDto, AwardCategory>();
        CreateMap<AwardCategoryUpdateDto, AwardCategory>();
    }
}