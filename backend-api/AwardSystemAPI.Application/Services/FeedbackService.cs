using AutoMapper;
using AwardSystemAPI.Application.DTOs;
using AwardSystemAPI.Common;
using AwardSystemAPI.Domain.Entities;
using AwardSystemAPI.Infrastructure.Repositories;
using Microsoft.Extensions.Logging;

namespace AwardSystemAPI.Application.Services;

public interface IFeedbackService
{
    Task<ApiResponse<IEnumerable<FeedbackFormQuestionResponseDto>, string>> GetFeedbackFormQuestionsAsync(int eventId);
    Task<ApiResponse<FeedbackFormQuestionResponseDto, string>> CreateFeedbackFormQuestionAsync(FeedbackFormQuestionCreateDto dto);
    Task<ApiResponse<bool, string>> UpdateFeedbackFormQuestionAsync(int questionId, FeedbackFormQuestionUpdateDto dto);
}

public class FeedbackService : IFeedbackService
{
    private readonly IFeedbackFormQuestionRepository _feedbackFormQuestionRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<FeedbackService> _logger;

    public FeedbackService(IFeedbackFormQuestionRepository feedbackFormQuestionRepository, IMapper mapper, ILogger<FeedbackService> logger)
    {
        _logger = logger;
        _feedbackFormQuestionRepository = feedbackFormQuestionRepository;
        _mapper = mapper;
    }
    public async Task<ApiResponse<IEnumerable<FeedbackFormQuestionResponseDto>, string>> GetFeedbackFormQuestionsAsync(int eventId)
    {
        var questions = await _feedbackFormQuestionRepository.GetByAwardCategory(eventId);
        var dtos = _mapper.Map<IEnumerable<FeedbackFormQuestionResponseDto>>(questions);
        return dtos.ToArray();
    }
    
    public async Task<ApiResponse<FeedbackFormQuestionResponseDto, string>> CreateFeedbackFormQuestionAsync(FeedbackFormQuestionCreateDto dto)
    {
        if (dto == null) throw new ArgumentNullException(nameof(dto));

        var entity = _mapper.Map<FeedbackFormQuestion>(dto);

        await _feedbackFormQuestionRepository.AddAsync(entity);
        _logger.LogInformation("Created FeedbackFormQuestion with ID {Id}.", entity.Id);

        var responseDto = _mapper.Map<FeedbackFormQuestionResponseDto>(entity);
        return responseDto;
    }
    
     public async Task<ApiResponse<bool, string>> UpdateFeedbackFormQuestionAsync(int questionId,
        FeedbackFormQuestionUpdateDto dto)
    {
        var question = await _feedbackFormQuestionRepository.GetByIdAsync(questionId);
        if (question == null)
        {
            _logger.LogWarning("FeedbackFormQuestion with ID {Id} not found.", questionId);
            return $"FeedbackFormQuestion with ID {questionId} not found.";
        }

        _mapper.Map(dto, question);

        await _feedbackFormQuestionRepository.UpdateAsync(question);
        _logger.LogInformation("Updated FeedbackFormQuestion with ID {Id}.", question.Id);

        return true;
    }
}