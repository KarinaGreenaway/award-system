using AutoMapper;
using AwardSystemAPI.Application.DTOs;
using AwardSystemAPI.Domain.Entities;

namespace AwardSystemAPI.Application.Mappings;

public class RsvpProfile: Profile
{
    public RsvpProfile()
    {
        CreateMap<RsvpFormQuestion, RsvpFormQuestionResponseDto>()
            .ReverseMap();

        CreateMap<RsvpFormQuestionCreateDto, RsvpFormQuestion>()
            .ReverseMap();

        CreateMap<RsvpFormQuestionUpdateDto, RsvpFormQuestion>()
            .ReverseMap();
    }
}