using AwardSystemAPI.Application.DTOs;
using AwardSystemAPI.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AwardSystemAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RsvpController: ControllerBase
{
    private readonly IRsvpService _rsvpService;
    private readonly ILogger<RsvpController> _logger;
    
    public RsvpController(IRsvpService rsvpService, ILogger<RsvpController> logger)
    {
        _rsvpService = rsvpService;
        _logger = logger;
    }
    
    [HttpGet("{awardEventId:int}/questions")]
    public async Task<ActionResult<IEnumerable<RsvpFormQuestionResponseDto>>> GetRsvpFormQuestions(int awardEventId)
    {
        if (awardEventId <= 0)
        {
            _logger.LogWarning("Invalid AwardEvent ID {Id} provided.", awardEventId);
            return BadRequest(new { Error = "Invalid AwardEvent ID provided." });
        }

        var response = await _rsvpService.GetRsvpFormQuestionsByAwardCategoryAsync(awardEventId);
        return response.Match<ActionResult>(
            onSuccess: result => Ok(result),
            onError: error =>
            {
                _logger.LogError("Failed to retrieve Rsvp form questions for AwardEvent ID {Id}. Error: {Error}", awardEventId, error);
                return error.ToLower().Contains("not found")
                    ? NotFound(new { Error = error })
                    : BadRequest(new { Error = error });
            }
        );
    }
    
    [Authorize(Policy = "AdminOnlyPolicy")]
    [HttpPost("question")]
    public async Task<ActionResult<RsvpFormQuestionResponseDto>> CreateRsvpFormQuestion([FromBody] RsvpFormQuestionCreateDto dto)
    {
        if (dto == null)
        {
            _logger.LogWarning("Invalid Rsvp form question data provided.");
            return BadRequest(new { Error = "Invalid Rsvp form question data provided." });
        }

        var response = await _rsvpService.CreateRsvpFormQuestionAsync(dto);
        return response.Match<ActionResult>(
            onSuccess: result => CreatedAtAction(nameof(GetRsvpFormQuestions), new { awardEventId = dto.EventId }, result),
            onError: error =>
            {
                _logger.LogError("Failed to create Rsvp form question. Error: {Error}", error);
                return BadRequest(new { Error = error });
            }
        );
    }
    
    [Authorize(Policy = "AdminOnlyPolicy")]
    [HttpPut("question/{questionId:int}")]
    public async Task<ActionResult<bool>> UpdateRsvpFormQuestion(int questionId, [FromBody] RsvpFormQuestionUpdateDto dto)
    {
        if (questionId <= 0)
        {
            _logger.LogWarning("Invalid Question ID {Id} provided.", questionId);
            return BadRequest(new { Error = "Invalid Question ID provided." });
        }

        var response = await _rsvpService.UpdateRsvpFormQuestionAsync(questionId, dto);
        return response.Match<ActionResult>(
            onSuccess: result => Ok(result),
            onError: error =>
            {
                _logger.LogError("Failed to update Rsvp form question with ID {Id}. Error: {Error}", questionId, error);
                return error.ToLower().Contains("not found")
                    ? NotFound(new { Error = error })
                    : BadRequest(new { Error = error });
            }
        );
    }
    [Authorize(Policy = "AdminOnlyPolicy")]
    [HttpDelete("question/{questionId:int}")]
    public async Task<ActionResult<bool>> DeleteRsvpFormQuestion(int questionId)
    {
        if (questionId <= 0)
        {
            _logger.LogWarning("Invalid Question ID {Id} provided.", questionId);
            return BadRequest(new { Error = "Invalid Question ID provided." });
        }

        var response = await _rsvpService.DeleteRsvpFormQuestionAsync(questionId);
        return response.Match<ActionResult>(
            onSuccess: result => Ok(result),
            onError: error =>
            {
                _logger.LogError("Failed to delete Rsvp form question with ID {Id}. Error: {Error}", questionId, error);
                return error.ToLower().Contains("not found")
                    ? NotFound(new { Error = error })
                    : BadRequest(new { Error = error });
            }
        );
    }
    
}