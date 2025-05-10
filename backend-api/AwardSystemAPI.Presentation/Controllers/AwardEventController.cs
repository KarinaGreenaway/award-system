using AwardSystemAPI.Application.DTOs;
using AwardSystemAPI.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AwardSystemAPI.Controllers;

[ApiController]
[Authorize(Policy = "AdminOnlyPolicy")]
[Route("api/[controller]")]
public class AwardEventController : ControllerBase
{
    private readonly IAwardEventService _awardEventService;
    private readonly ILogger<AwardEventController> _logger;

    public AwardEventController(IAwardEventService awardEventService, ILogger<AwardEventController> logger)
    {
        _awardEventService = awardEventService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<AwardEventResponseDto>>> GetAll()
    {
        var response = await _awardEventService.GetAllAwardEventsAsync();
        return response.Match<ActionResult>(
            onSuccess: result => Ok(result),
            onError: error =>
            {
                _logger.LogError("Failed to retrieve AwardEvents. Error: {Error}", error);
                return BadRequest(new { Error = error });
            }
        );
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<AwardEventResponseDto>> GetById(int id)
    {
        if (id <= 0)
        {
            _logger.LogWarning("Invalid AwardEvent ID {Id} provided.", id);
            return BadRequest(new { Error = "Invalid AwardEvent ID provided." });
        }

        var response = await _awardEventService.GetAwardEventByIdAsync(id);
        return response.Match<ActionResult>(
            onSuccess: result => Ok(result),
            onError: error =>
            {
                _logger.LogError("Failed to retrieve AwardEvent with ID {Id}. Error: {Error}", id, error);
                return error.ToLower().Contains("not found")
                    ? NotFound(new { Error = error })
                    : BadRequest(new { Error = error });
            }
        );
    }
    
    [HttpGet ("awardProcess/{id:int}")]
    public async Task<ActionResult<AwardEventResponseDto>> GetByAwardProcessId(int id)
    {
        if (id <= 0)
        {
            _logger.LogWarning("Invalid AwardProcess ID {Id} provided.", id);
            return BadRequest(new { Error = "Invalid AwardProcess ID provided." });
        }

        var response = await _awardEventService.GetAwardEventByAwardProcessIdAsync(id);
        return response.Match<ActionResult>(
            onSuccess: result => Ok(result),
            onError: error =>
            {
                _logger.LogError("Failed to retrieve AwardEvent with ID {Id}. Error: {Error}", id, error);
                return error.ToLower().Contains("not found")
                    ? NotFound(new { Error = error })
                    : BadRequest(new { Error = error });
            }
        );
    }

    [HttpPost]
    public async Task<ActionResult<AwardEventResponseDto>> Create([FromBody] AwardEventCreateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var response = await _awardEventService.CreateAwardEventAsync(dto);
        return response.Match<ActionResult>(
            onSuccess: created =>
            {
                _logger.LogInformation("Created AwardEvent with ID {Id}.", created.Id);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            },
            onError: error =>
            {
                _logger.LogError("Failed to create AwardEvent. Error: {Error}", error);
                return BadRequest(new { Error = error });
            }
        );
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] AwardEventUpdateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var response = await _awardEventService.UpdateAwardEventAsync(id, dto);
        return response.Match<IActionResult>(
            onSuccess: _ =>
            {
                _logger.LogInformation("Updated AwardEvent with ID {Id}.", id);
                return Ok();
            },
            onError: error =>
            {
                _logger.LogError("Failed to update AwardEvent with ID {Id}. Error: {Error}", id, error);
                return error.ToLower().Contains("not found")
                    ? NotFound(new { Error = error })
                    : BadRequest(new { Error = error });
            }
        );
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var response = await _awardEventService.DeleteAwardEventAsync(id);
        return response.Match<IActionResult>(
            onSuccess: _ =>
            {
                _logger.LogInformation("Deleted AwardEvent with ID {Id}.", id);
                return Ok();
            },
            onError: error =>
            {
                _logger.LogError("Failed to delete AwardEvent with ID {Id}. Error: {Error}", id, error);
                return error.ToLower().Contains("not found")
                    ? NotFound(new { Error = error })
                    : BadRequest(new { Error = error });
            }
        );
    }
}