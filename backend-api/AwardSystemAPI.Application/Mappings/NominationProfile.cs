using AutoMapper;
using AwardSystemAPI.Application.DTOs;
using AwardSystemAPI.Domain.Entities;

namespace AwardSystemAPI.Application.Mappings;

public class NominationProfile: Profile
{
    public NominationProfile()
    {
        CreateMap<NominationCreateDto, Nomination>();
        CreateMap<NominationUpdateDto, Nomination>();
        CreateMap<Nomination, NominationResponseDto>().ReverseMap();

        CreateMap<NominationAnswerCreateDto, NominationAnswer>();
        CreateMap<NominationAnswerUpdateDto, NominationAnswer>();
        CreateMap<NominationAnswer, NominationAnswerResponseDto>().ReverseMap();
    }
}