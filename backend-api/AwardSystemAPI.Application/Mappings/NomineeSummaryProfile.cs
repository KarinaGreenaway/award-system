using AutoMapper;
using AwardSystemAPI.Application.DTOs;
using AwardSystemAPI.Domain.Entities;

namespace AwardSystemAPI.Application.Mappings;

public class NomineeSummaryProfile: Profile
{
    public NomineeSummaryProfile()
    {
        CreateMap<NomineeSummary, NomineeSummaryResponseDto>().ReverseMap();
        
        CreateMap<NomineeSummary, NomineeSummaryCreateDto>().ReverseMap()
            .ForMember(dest => dest.Id, opt => opt.Ignore());;
        
        CreateMap<NomineeSummary, NomineeSummaryUpdateDto>().ReverseMap();
        
        CreateMap<NomineeSummary, NomineeSummaryWithUserDto>()
            .ForMember(dest => dest.NomineeName, opt => opt.MapFrom(src => src.Nominee.DisplayName));

    }
    
}