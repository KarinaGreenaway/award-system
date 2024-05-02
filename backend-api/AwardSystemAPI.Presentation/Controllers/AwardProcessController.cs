using AwardSystemAPI.Application.DTOs.AwardProcessDtos;
using AwardSystemAPI.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AwardSystemAPI.Controllers;

[ApiController]
[Authorize(Policy = "AdminOnlyPolicy")]
[Route("api/[controller]")]
public class AwardProcessController : ControllerBase
{
    private readonly IAwardProcessService _awardProcessService;
    private readonly ILogger<AwardProcessController> _logger;

    public AwardProcessController(IAwardProcessService awardProcessService, ILogger<AwardProcessController> logger)
    {
        _awardProcessService = awardProcessService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<AwardProcessResponseDto>>> GetAll()
    {
        var response = await _awardProcessService.GetAllAwardProcessesAsync();
        return response.Match<ActionResult>(
            onSuccess: result => Ok(result),
            onError: error =>
            {
                _logger.LogError("Failed to retrieve AwardProcesses. Error: {Error}", error);
                return BadRequest(new { Error = error });
            }
        );
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<AwardProcessResponseDto>> GetById(int id)
    {
        if (id <= 0)
        {
            _logger.LogWarning("Invalid ID {Id} provided for GetById.", id);
            return BadRequest(new { Error = "Invalid ID provided." });
        }

        var response = await _awardProcessService.GetAwardProcessByIdAsync(id);
        return response.Match<ActionResult>(
            onSuccess: result => Ok(result),
            onError: error =>
            {
                _logger.LogError("Failed to retrieve AwardProcess with ID {Id}. Error: {Error}", id, error);
                return error.ToLower().Contains("not found")
                    ? NotFound(new { Error = error })
                    : BadRequest(new { Error = error });
            }
        );
    }
    
    [HttpGet("active")]
    public async Task<ActionResult<AwardProcessResponseDto>> GetActive()
    {
        var response = await _awardProcessService.GetActiveAwardProcessesAsync();
        return response.Match<ActionResult>(
            onSuccess: result => Ok(result),
            onError: error =>
            {
                _logger.LogError("Failed to retrieve active AwardProcess. Error: {Error}", error);
                return BadRequest(new { Error = error });
            }
        );
    }

    [HttpPost]
    public async Task<ActionResult<AwardProcessResponseDto>> Create([FromBody] AwardProcessCreateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var response = await _awardProcessService.CreateAwardProcessAsync(dto);

        return response.Match<ActionResult>(
            onSuccess: created =>
            {
                _logger.LogInformation("Successfully created AwardProcess with ID {Id}.", created.Id);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            },
            onError: error =>
            {
                _logger.LogError("Failed to create AwardProcess. Error: {Error}", error);
                return BadRequest(new { Error = error });
            }
        );
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] AwardProcessUpdateDto dto)
    {
        if (id <= 0)
            return BadRequest(new { Error = "Invalid ID provided." });

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var response = await _awardProcessService.UpdateAwardProcessAsync(id, dto);

        return response.Match<IActionResult>(
            onSuccess: _ =>
            {
                _logger.LogInformation("Successfully updated AwardProcess with ID {Id}.", id);
                return Ok();
            },
            onError: error =>
            {
                _logger.LogError("Failed to update AwardProcess with ID {Id}. Error: {Error}", id, error);
                return error.ToLower().Contains("not found")
                    ? NotFound(new { Error = error })
                    : BadRequest(new { Error = error });
            }
        );
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        if (id <= 0)
            return BadRequest(new { Error = "Invalid ID provided." });

        var response = await _awardProcessService.DeleteAwardProcessAsync(id);

        return response.Match<IActionResult>(
            onSuccess: _ =>
            {
                _logger.LogInformation("Successfully deleted AwardProcess with ID {Id}.", id);
                return Ok();
            },
            onError: error =>
            {
                _logger.LogError("Failed to delete AwardProcess with ID {Id}. Error: {Error}", id, error);
                return error.ToLower().Contains("not found")
                    ? NotFound(new { Error = error })
                    : BadRequest(new { Error = error });
            }
        );
    }
}
