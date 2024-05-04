using System.Text.Json;
using AutoMapper;
using AwardSystemAPI.Application.DTOs;
using AwardSystemAPI.Domain.Entities;

namespace AwardSystemAPI.Application.Mappings;

public class NominationQuestionProfile : Profile
{
    public NominationQuestionProfile()
    {
        CreateMap<NominationQuestionCreateDto, NominationQuestion>();

        CreateMap<NominationQuestionUpdateDto, NominationQuestion>();

        CreateMap<NominationQuestion, NominationQuestionResponseDto>();
    }
}
