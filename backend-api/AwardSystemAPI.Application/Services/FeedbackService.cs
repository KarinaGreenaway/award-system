using AutoMapper;
using AwardSystemAPI.Application.DTOs;
using AwardSystemAPI.Common;
using AwardSystemAPI.Domain.Entities;
using AwardSystemAPI.Infrastructure.Repositories;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace AwardSystemAPI.Application.Services;

public interface IFeedbackService
{
    Task<ApiResponse<IEnumerable<FeedbackResponseDto>, string>> GetFeedbackAsync(int eventId);
    Task<string> GetFeedbackSummaryAsync(int eventId);
    Task<ApiResponse<FeedbackResponseDto, string>> CreateFeedbackAsync(FeedbackCreateDto dto);
    Task<ApiResponse<FeedbackAnalyticsResponseDto, string>> GetFeedbackAnalyticsAsync(int eventId);
    Task<ApiResponse<IEnumerable<FeedbackFormQuestionResponseDto>, string>> GetFeedbackFormQuestionsAsync(int eventId);
    Task<ApiResponse<FeedbackFormQuestionResponseDto, string>> CreateFeedbackFormQuestionAsync(FeedbackFormQuestionCreateDto dto);
    Task<ApiResponse<bool, string>> UpdateFeedbackFormQuestionAsync(int questionId, FeedbackFormQuestionUpdateDto dto);
}

public class FeedbackService : IFeedbackService
{
    private readonly IFeedbackFormQuestionRepository _feedbackFormQuestionRepository;
    private readonly IFeedbackRepository _feedbackRepository;
    private readonly IRsvpRepository _rsvpRepository;
    private readonly IAiSummaryService _aiSummaryService;
    private readonly IMapper _mapper;
    private readonly ILogger<FeedbackService> _logger;

    public FeedbackService(IFeedbackRepository feedbackRepository,
        IFeedbackFormQuestionRepository feedbackFormQuestionRepository,
        IRsvpRepository rsvpRepository,
        IAiSummaryService aiSummaryService,
        IMapper mapper,
        ILogger<FeedbackService> logger)
    {
        _logger = logger;
        _feedbackRepository = feedbackRepository;
        _rsvpRepository = rsvpRepository;
        _aiSummaryService = aiSummaryService;
        _feedbackFormQuestionRepository = feedbackFormQuestionRepository;
        _mapper = mapper;
    }
    
    public async Task<ApiResponse<IEnumerable<FeedbackResponseDto>, string>> GetFeedbackAsync(int eventId)
    {
        var feedbacks = await _feedbackRepository.GetByAwardEventId(eventId);
        var dtos = _mapper.Map<IEnumerable<FeedbackResponseDto>>(feedbacks);
        return dtos.ToArray();
    }
    
    public async Task<string> GetFeedbackSummaryAsync(int eventId)
    {
        var allFeedbacks = await _feedbackRepository.GetByAwardEventId(eventId);
        var feedbackJson = JsonConvert.SerializeObject(allFeedbacks);
        var summary = await _aiSummaryService.GenerateAiFeedbackSummaryAsync(feedbackJson);
        
        if (string.IsNullOrEmpty(summary))
        {
            _logger.LogWarning("Failed to generate feedback summary for AwardEvent ID {Id}.", eventId);
            return "Failed to generate feedback summary.";
        }
        _logger.LogInformation("Generated feedback summary for AwardEvent ID {Id}.", eventId);
        return summary;
    }
    
    public async Task<ApiResponse<FeedbackResponseDto, string>> CreateFeedbackAsync(FeedbackCreateDto dto)
    {
        if (dto == null) throw new ArgumentNullException(nameof(dto));

        var entity = _mapper.Map<Feedback>(dto);
        var answers = _mapper.Map<IEnumerable<FeedbackResponse>>(dto.Answers);
        
        var feedbackAnswers = answers as FeedbackResponse[] ?? answers.ToArray();
        entity.Answers = feedbackAnswers;

        await _feedbackRepository.AddAsync(entity);
        _logger.LogInformation("Created Feedback with ID {Id}.", entity.Id);

        var responseDto = _mapper.Map<FeedbackResponseDto>(entity);
        return responseDto;
    }
    
    public async Task<ApiResponse<FeedbackAnalyticsResponseDto, string>> GetFeedbackAnalyticsAsync(int eventId)
    {
        var totalFeedbacks = await _feedbackRepository.CountByAwardEventId(eventId);
        var totalRsvps = await _rsvpRepository.CountByAwardEventId(eventId);

        double responseRate = 0;
        if (totalRsvps > 0)
        {
            responseRate = (double)totalFeedbacks / totalRsvps * 100;
        }

        var analytics = new FeedbackAnalyticsResponseDto
        {
            TotalFeedbacks = totalFeedbacks,
            ResponseRate = Math.Round(responseRate, 2)
        };

        return analytics;
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