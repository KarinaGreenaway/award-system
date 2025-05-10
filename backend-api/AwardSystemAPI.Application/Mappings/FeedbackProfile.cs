using AutoMapper;
using AwardSystemAPI.Application.DTOs;
using AwardSystemAPI.Domain.Entities;

namespace AwardSystemAPI.Application.Mappings;

public class FeedbackProfile: Profile
{
    public FeedbackProfile()
    {
        CreateMap<FeedbackCreateDto, Feedback>();
        CreateMap<FeedbackUpdateDto, Feedback>();
        
        CreateMap<Feedback, FeedbackResponseDto>()
            .ForMember(dest => dest.Answers,
                opt => opt.MapFrom(src => src.Answers));

        CreateMap<FeedbackAnswerUpdateDto, FeedbackResponse>();
        CreateMap<FeedbackAnswerResponseDto, FeedbackResponse>().ReverseMap();
        CreateMap<FeedbackAnswerCreateDto, FeedbackResponse>().ReverseMap();

    }
}
