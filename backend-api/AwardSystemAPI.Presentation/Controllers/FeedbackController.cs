using AwardSystemAPI.Application.DTOs;
using AwardSystemAPI.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AwardSystemAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FeedbackController : ControllerBase
{
    private readonly IFeedbackService _feedbackService;
    private readonly ILogger<FeedbackController> _logger;

    public FeedbackController(IFeedbackService feedbackService, ILogger<FeedbackController> logger)
    {
        _feedbackService = feedbackService;
        _logger = logger;
    }
    
    [HttpGet("{awardEventId:int}")]
    public async Task<ActionResult<IEnumerable<FeedbackResponseDto>>> GetFeedback(int awardEventId)
    {
        if (awardEventId <= 0)
        {
            _logger.LogWarning("Invalid AwardEvent ID {Id} provided.", awardEventId);
            return BadRequest(new { Error = "Invalid AwardEvent ID provided." });
        }

        var response = await _feedbackService.GetFeedbackAsync(awardEventId);
        return response.Match<ActionResult>(
            onSuccess: result => Ok(result),
            onError: error =>
            {
                _logger.LogError("Failed to retrieve Feedbacks for AwardEvent ID {Id}. Error: {Error}",
                    awardEventId, error);
                return error.ToLower().Contains("not found")
                    ? NotFound(new { Error = error })
                    : BadRequest(new { Error = error });
            }
        );
    }
    
    [HttpGet("summary/{awardEventId:int}")]
    public async Task<ActionResult<string>> GetFeedbackSummary(int awardEventId)
    {
        if (awardEventId <= 0)
        {
            _logger.LogWarning("Invalid AwardEvent ID {Id} provided.", awardEventId);
            return BadRequest(new { Error = "Invalid AwardEvent ID provided." });
        }

        var response = await _feedbackService.GetFeedbackSummaryAsync(awardEventId);
        return response;
    }
    
    [HttpPost]
    public async Task<ActionResult<FeedbackResponseDto>> CreateFeedback([FromBody] FeedbackCreateDto dto)
    {
        var response = await _feedbackService.CreateFeedbackAsync(dto);
        return response.Match<ActionResult>(
            onSuccess: result => CreatedAtAction(nameof(GetFeedback), new { awardEventId = dto.EventId }, result),
            onError: error =>
            {
                _logger.LogError("Failed to create Feedback. Error: {Error}", error);
                return BadRequest(new { Error = error });
            }
        );
    }
    
    // get feedback analytics
    [HttpGet("analytics/{awardEventId:int}")]
    public async Task<ActionResult<FeedbackAnalyticsResponseDto>> GetFeedbackAnalytics(int awardEventId)
    {
        if (awardEventId <= 0)
        {
            _logger.LogWarning("Invalid AwardEvent ID {Id} provided.", awardEventId);
            return BadRequest(new { Error = "Invalid AwardEvent ID provided." });
        }

        var response = await _feedbackService.GetFeedbackAnalyticsAsync(awardEventId);
        return response.Match<ActionResult>(
            onSuccess: result => Ok(result),
            onError: error =>
            {
                _logger.LogError("Failed to retrieve Feedback analytics for AwardEvent ID {Id}. Error: {Error}",
                    awardEventId, error);
                return error.ToLower().Contains("not found")
                    ? NotFound(new { Error = error })
                    : BadRequest(new { Error = error });
            }
        );
    }

    [HttpGet("{awardEventId:int}/questions")]
    public async Task<ActionResult<IEnumerable<FeedbackFormQuestionResponseDto>>> GetFeedbackFormQuestions(
        int awardEventId)
    {
        if (awardEventId <= 0)
        {
            _logger.LogWarning("Invalid AwardEvent ID {Id} provided.", awardEventId);
            return BadRequest(new { Error = "Invalid AwardEvent ID provided." });
        }

        var response = await _feedbackService.GetFeedbackFormQuestionsAsync(awardEventId);
        return response.Match<ActionResult>(
            onSuccess: result => Ok(result),
            onError: error =>
            {
                _logger.LogError("Failed to retrieve Feedback form questions for AwardEvent ID {Id}. Error: {Error}",
                    awardEventId, error);
                return error.ToLower().Contains("not found")
                    ? NotFound(new { Error = error })
                    : BadRequest(new { Error = error });
            }
        );
    }
    
    [Authorize(Policy = "AdminOnlyPolicy")]
    [HttpPost("question")]
    public async Task<ActionResult<FeedbackFormQuestionResponseDto>> CreateFeedbackFormQuestion(
        [FromBody] FeedbackFormQuestionCreateDto dto)
    {
        if (dto == null)
        {
            _logger.LogWarning("Invalid Feedback form question data provided.");
            return BadRequest(new { Error = "Invalid Feedback form question data provided." });
        }

        var response = await _feedbackService.CreateFeedbackFormQuestionAsync(dto);
        return response.Match<ActionResult>(
            onSuccess: result => CreatedAtAction(nameof(GetFeedbackFormQuestions), new { awardEventId = dto.EventId }, result),
            onError: error =>
            {
                _logger.LogError("Failed to create Feedback form question. Error: {Error}", error);
                return BadRequest(new { Error = error });
            }
        );
    }
    
    [Authorize(Policy = "AdminOnlyPolicy")]
    [HttpPut("question/{questionId:int}")]
    public async Task<ActionResult<bool>> UpdateFeedbackFormQuestion(int questionId,
        [FromBody] FeedbackFormQuestionUpdateDto dto)
    {
        if (questionId <= 0)
        {
            _logger.LogWarning("Invalid Feedback form question ID {Id} provided.", questionId);
            return BadRequest(new { Error = "Invalid Feedback form question ID provided." });
        }

        var response = await _feedbackService.UpdateFeedbackFormQuestionAsync(questionId, dto);
        return response.Match<ActionResult>(
            onSuccess: result => Ok(result),
            onError: error =>
            {
                _logger.LogError("Failed to update Feedback form question. Error: {Error}", error);
                return BadRequest(new { Error = error });
            }
        );
    }
}
