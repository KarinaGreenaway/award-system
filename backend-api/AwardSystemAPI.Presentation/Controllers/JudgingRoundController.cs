using AwardSystemAPI.Application.Services;
using Microsoft.AspNetCore.Mvc;
using AwardSystemAPI.Application.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace AwardSystemAPI.Controllers;

[ApiController]
[Authorize(Policy = "AdminOnlyPolicy")]
[Route("api/[controller]")]
public class JudgingRoundController : ControllerBase
{
    private readonly IJudgingRoundService _service;
    private readonly ILogger<JudgingRoundController> _logger;

    public JudgingRoundController(IJudgingRoundService service, ILogger<JudgingRoundController> logger)
    {
        _service = service;
        _logger = logger;
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<JudgingRoundResponseDto>> GetById(int id)
    {
        if (id <= 0)
        {
            _logger.LogWarning("Invalid JudgingRound ID {Id} provided.", id);
            return BadRequest(new { Error = "Invalid JudgingRound ID provided." });
        }

        var response = await _service.GetJudgingRoundByIdAsync(id);
        return response.Match<ActionResult>(
            onSuccess: dto => Ok(dto),
            onError: error =>
            {
                _logger.LogError("Failed to retrieve JudgingRound with ID {Id}. Error: {Error}", id, error);
                return error.ToLower().Contains("not found")
                    ? NotFound(new { Error = error })
                    : BadRequest(new { Error = error });
            });
    }

    [HttpGet("awardprocess/{awardProcessId:int}")]
    public async Task<ActionResult<IEnumerable<JudgingRoundResponseDto>>> GetByAwardProcess(int awardProcessId)
    {
        var response = await _service.GetJudgingRoundsByAwardProcessIdAsync(awardProcessId);
        return response.Match<ActionResult>(
            onSuccess: dtos => Ok(dtos),
            onError: error =>
            {
                _logger.LogError("Failed to retrieve JudgingRounds for AwardProcess ID {AwardProcessId}. Error: {Error}", awardProcessId, error);
                return BadRequest(new { Error = error });
            });
    }

    [HttpPost]
    public async Task<ActionResult<JudgingRoundResponseDto>> Create([FromBody] JudgingRoundCreateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var response = await _service.CreateJudgingRoundAsync(dto);
        return response.Match<ActionResult>(
            onSuccess: created =>
            {
                _logger.LogInformation("Created JudgingRound with ID {Id}.", created.Id);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            },
            onError: error =>
            {
                _logger.LogError("Failed to create JudgingRound. Error: {Error}", error);
                return BadRequest(new { Error = error });
            });
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] JudgingRoundUpdateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var response = await _service.UpdateJudgingRoundAsync(id, dto);
        return response.Match<IActionResult>(
            onSuccess: _ =>
            {
                _logger.LogInformation("Updated JudgingRound with ID {Id}.", id);
                return Ok();
            },
            onError: error =>
            {
                _logger.LogError("Failed to update JudgingRound with ID {Id}. Error: {Error}", id, error);
                return error.ToLower().Contains("not found")
                    ? NotFound(new { Error = error })
                    : BadRequest(new { Error = error });
            });
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var response = await _service.DeleteJudgingRoundAsync(id);
        return response.Match<IActionResult>(
            onSuccess: _ =>
            {
                _logger.LogInformation("Deleted JudgingRound with ID {Id}.", id);
                return Ok();
            },
            onError: error =>
            {
                _logger.LogError("Failed to delete JudgingRound with ID {Id}. Error: {Error}", id, error);
                return error.ToLower().Contains("not found")
                    ? NotFound(new { Error = error })
                    : BadRequest(new { Error = error });
            });
    }
}