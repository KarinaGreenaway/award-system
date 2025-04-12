using AutoMapper;
using AwardSystemAPI.Application.DTOs;
using AwardSystemAPI.Domain.Entities;

namespace AwardSystemAPI.Application.Mappings;

public class NomineeSummaryProfile: Profile
{
    public NomineeSummaryProfile()
    {
        CreateMap<NomineeSummary, NomineeSummaryResponseDto>().ReverseMap();
        
        CreateMap<NomineeSummary, NomineeSummaryCreateDto>().ReverseMap();
        
        CreateMap<NomineeSummary, NomineeSummaryUpdateDto>().ReverseMap();
    }
    
}