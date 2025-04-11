using AutoMapper;
using AwardSystemAPI.Application.DTOs;
using AwardSystemAPI.Domain.Entities;

namespace AwardSystemAPI.Application.Mappings;

public class AwardEventProfile: Profile
{
    public AwardEventProfile()
    {
        CreateMap<AwardEventCreateDto, AwardEvent>();
        CreateMap<AwardEventUpdateDto, AwardEvent>();
        CreateMap<AwardEvent, AwardEventResponseDto>();
    }
}