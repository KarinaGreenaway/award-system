using AutoMapper;
using AwardSystemAPI.Application.DTOs;
using AwardSystemAPI.Domain.Entities;

namespace AwardSystemAPI.Application.Mappings;

public class FeedbackFormQuestionProfile: Profile
{
    public FeedbackFormQuestionProfile()
    {
        CreateMap<FeedbackFormQuestion, FeedbackFormQuestionResponseDto>()
            .ReverseMap();

        CreateMap<FeedbackFormQuestionCreateDto, FeedbackFormQuestion>()
            .ReverseMap();

        CreateMap<FeedbackFormQuestionUpdateDto, FeedbackFormQuestion>()
            .ReverseMap();
    }
}