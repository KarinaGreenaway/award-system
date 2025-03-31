using AwardSystemAPI.Application.DTOs;
using AwardSystemAPI.Application.Services;
using Microsoft.AspNetCore.Mvc;
namespace AwardSystemAPI.Controllers;

[ApiController]
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

    [HttpGet("get-all")]
    public async Task<IActionResult> GetAll()
    {
        var processes = await _awardProcessService.GetAllAwardProcessesAsync();
        return Ok(processes);
    }

    [HttpGet("get-{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var process = await _awardProcessService.GetAwardProcessByIdAsync(id);
        if (process == null)
        {
            _logger.LogWarning("AwardProcess with ID {Id} not found.", id);
            return NotFound();
        }
        return Ok(process);
    }

    [HttpPost("add")]
    public async Task<IActionResult> Create([FromBody] AwardProcessCreateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var createdProcess = await _awardProcessService.CreateAwardProcessAsync(dto);
        _logger.LogInformation("Created AwardProcess with ID {Id}.", createdProcess.Id);
        return CreatedAtAction(nameof(GetById), new { id = createdProcess.Id }, createdProcess);
    }

    [HttpPut("update-{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] AwardProcessUpdateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var updated = await _awardProcessService.UpdateAwardProcessAsync(id, dto);
        if (!updated)
        {
            _logger.LogWarning("AwardProcess with ID {Id} not found for update.", id);
            return NotFound();
        }
        _logger.LogInformation("Updated AwardProcess with ID {Id}.", id);
        return NoContent();
    }

    [HttpDelete("delete-{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _awardProcessService.DeleteAwardProcessAsync(id);
        if (!deleted)
        {
            _logger.LogWarning("AwardProcess with ID {Id} not found for deletion.", id);
            return NotFound();
        }
        _logger.LogInformation("Deleted AwardProcess with ID {Id}.", id);
        return NoContent();
    }
}