using AutoMapper;
using AwardSystemAPI.Application.DTOs;
using AwardSystemAPI.Domain.Entities;

namespace AwardSystemAPI.Application.Mappings;

public class NominationProfile : Profile
{
    public NominationProfile()
    {
        CreateMap<NominationCreateDto, Nomination>();
        CreateMap<NominationUpdateDto, Nomination>();

        CreateMap<NominationAnswerCreateDto, NominationAnswer>();
        CreateMap<NominationAnswerUpdateDto, NominationAnswer>();
        CreateMap<NominationAnswer, NominationAnswerResponseDto>().ReverseMap();

        CreateMap<Nomination, NominationResponseDto>()
            .ForMember(dest => dest.NomineeName,
                opt => opt.MapFrom(src => src.NomineeUser != null ? src.NomineeUser.DisplayName : null))
            .ForMember(dest => dest.TeamMembers,
                opt => opt.MapFrom(src => src.TeamMembers));
    }
}