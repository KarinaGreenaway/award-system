using AutoMapper;
using AwardSystemAPI.Application.DTOs;
using AwardSystemAPI.Domain.Entities;

namespace AwardSystemAPI.Application.Mappings;

public class AwardCategoryProfile: Profile
{
    public AwardCategoryProfile()
    {
        CreateMap<AwardCategory, AwardCategoryResponseDto>();
        CreateMap<AwardCategoryCreateDto, AwardCategory>();
        CreateMap<AwardCategoryUpdateDto, AwardCategory>();
    }
}