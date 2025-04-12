using AwardSystemAPI.Application.DTOs;
using AwardSystemAPI.Application.Services;
using AwardSystemAPI.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AwardSystemAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class NomineeSummaryController: ControllerBase
{
    private readonly INomineeSummaryService _nomineeSummaryService;
    private readonly ILogger<NomineeSummaryController> _logger;

    public NomineeSummaryController(INomineeSummaryService nomineeSummaryService, ILogger<NomineeSummaryController> logger)
    {
        _nomineeSummaryService = nomineeSummaryService;
        _logger = logger;
    }

    [HttpPost]
    public async Task<ActionResult<NomineeSummaryResponseDto>> Create([FromBody] NomineeSummaryCreateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var userId = User.GetUserId();
        if (userId == null)
        {
            _logger.LogWarning("User ID claim not found.");
            return Unauthorized("User ID is missing from the token.");
        }

        var response = await _nomineeSummaryService.CreateNomineeSummaryAsync(dto);
        return response.Match<ActionResult>(
            onSuccess: created =>
            {
                _logger.LogInformation("Created NomineeSummary with ID {Id}.", created.Id);
                return CreatedAtAction(nameof(Create), new { id = created.Id }, created);
            },
            onError: error =>
            {
                _logger.LogError("Failed to create NomineeSummary. Error: {Error}", error);
                return BadRequest(new { Error = error });
            }
        );
    }
    
    // [Authorize(Roles = "Sponsor,Admin")]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] NomineeSummaryUpdateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var response = await _nomineeSummaryService.UpdateNomineeSummaryAsync(id, dto);
        return response.Match<IActionResult>(
            onSuccess: _ =>
            {
                _logger.LogInformation("Updated NomineeSummary with ID {Id}.", id);
                return Ok();
            },
            onError: error =>
            {
                _logger.LogError("Failed to update NomineeSummary with ID {Id}. Error: {Error}", id, error);
                return error.Contains("not found")
                    ? NotFound(new { Error = error })
                    : BadRequest(new { Error = error });
            }
        );
    }
    
    // [Authorize(Roles = "Sponsor,Admin")]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<NomineeSummaryResponseDto>>> GetAll()
    {
        var response = await _nomineeSummaryService.GetAllNomineeSummariesAsync();
        return response.Match<ActionResult>(
            onSuccess: result => Ok(result),
            onError: error =>
            {
                _logger.LogError("Failed to retrieve NomineeSummaries. Error: {Error}", error);
                return BadRequest(new { Error = error });
            }
        );
    }
    
    // [Authorize(Roles = "Sponsor,Admin")]
    [HttpGet("category/{categoryId:int}")]
    public async Task<ActionResult<IEnumerable<NomineeSummaryResponseDto>>> GetByCategoryId(int categoryId)
    {
        var response = await _nomineeSummaryService.GetAllNomineeSummariesByCategoryIdAsync(categoryId);
        return response.Match<ActionResult>(
            onSuccess: result => Ok(result),
            onError: error =>
            {
                _logger.LogError("Failed to retrieve NomineeSummaries for category {CategoryId}. Error: {Error}", categoryId, error);
                return BadRequest(new { Error = error });
            }
        );
    }
}