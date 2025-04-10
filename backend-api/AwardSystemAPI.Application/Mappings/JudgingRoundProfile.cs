using AutoMapper;
using AwardSystemAPI.Application.DTOs;
using AwardSystemAPI.Domain.Entities;

namespace AwardSystemAPI.Application.Mappings;

public class JudgingRoundProfile: Profile
{
    public JudgingRoundProfile()
    {
        CreateMap<JudgingRoundCreateDto, JudgingRound>();
        CreateMap<JudgingRoundUpdateDto, JudgingRound>();
        CreateMap<JudgingRound, JudgingRoundResponseDto>();
    }
}