using AwardSystemAPI.Application.DTOs.AwardCategoryDtos;
using AwardSystemAPI.Application.Services;
using AwardSystemAPI.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AwardSystemAPI.Controllers;

[ApiController]
[Authorize(Policy = "SponsorOrAdminPolicy")]
[Route("api/[controller]")]
public class AwardCategoryController : ControllerBase
{
    private readonly IAwardCategoryService _awardCategoryService;
    private readonly ILogger<AwardCategoryController> _logger;

    public AwardCategoryController(IAwardCategoryService awardCategoryService, ILogger<AwardCategoryController> logger)
    {
        _awardCategoryService = awardCategoryService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<AwardCategoryResponseDto>>> GetAll()
    {
        var response = await _awardCategoryService.GetAllAwardCategoriesAsync();
        return response.Match<ActionResult>(
            onSuccess: result => Ok(result),
            onError: error =>
            {
                _logger.LogError("Failed to retrieve AwardCategories. Error: {Error}", error);
                return BadRequest(new { Error = error });
            }
        );
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<AwardCategoryResponseDto>> GetById(int id)
    {
        if (id <= 0)
        {
            _logger.LogWarning("Invalid ID {Id} provided for GetById.", id);
            return BadRequest(new { Error = "Invalid ID provided." });
        }

        var response = await _awardCategoryService.GetAwardCategoryByIdAsync(id);
        return response.Match<ActionResult>(
            onSuccess: result => Ok(result),
            onError: error =>
            {
                _logger.LogError("Failed to retrieve AwardCategory with ID {Id}. Error: {Error}", id, error);
                return error.Contains("not found")
                    ? NotFound(new { Error = error })
                    : BadRequest(new { Error = error });
            }
        );
    }

    // Returns categories for the currently authenticated user.
    [HttpGet("my")]
    public async Task<ActionResult<IEnumerable<AwardCategoryResponseDto>>> GetMyCategories()
    {
        int? userId = User.GetUserId();
        if (userId == null)
        {
            _logger.LogWarning("User ID claim not found.");
            return Unauthorized("User ID is missing from the token.");
        }

        var response = await _awardCategoryService.GetAwardCategoriesBySponsorIdAsync(userId.Value);
        return response.Match<ActionResult>(
            onSuccess: result => Ok(result),
            onError: error =>
            {
                _logger.LogError("Failed to retrieve AwardCategories for sponsor {SponsorId}. Error: {Error}", userId, error);
                return BadRequest(new { Error = error });
            }
        );
    }

    [HttpPost]
    [Authorize(Policy = "CategoryOwnerPolicy")]
    public async Task<ActionResult<AwardCategoryResponseDto>> Create([FromBody] AwardCategoryCreateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var userId = User.GetUserId();
        if (userId == null)
        {
            _logger.LogWarning("User ID claim not found.");
            return Unauthorized("User ID is missing from the token.");
        }
        dto.SponsorId = userId.Value;

        var response = await _awardCategoryService.CreateAwardCategoryAsync(dto);

        return response.Match<ActionResult>(
            onSuccess: created =>
            {
                _logger.LogInformation("Created AwardCategory with ID {Id}.", created.Id);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            },
            onError: error =>
            {
                _logger.LogError("Failed to create AwardCategory. Error: {Error}", error);
                return BadRequest(new { Error = error });
            }
        );
    }

    [HttpPut("{id:int}")]
    [Authorize(Policy = "CategoryOwnerPolicy")]
    public async Task<IActionResult> Update(int id, [FromBody] AwardCategoryUpdateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var response = await _awardCategoryService.UpdateAwardCategoryAsync(id, dto);
        return response.Match<IActionResult>(
            onSuccess: _ =>
            {
                _logger.LogInformation("Updated AwardCategory with ID {Id}.", id);
                return Ok();
            },
            onError: error =>
            {
                _logger.LogError("Failed to update AwardCategory with ID {Id}. Error: {Error}", id, error);
                return error.Contains("not found")
                    ? NotFound(new { Error = error })
                    : BadRequest(new { Error = error });
            }
        );
    }

    [HttpDelete("{id:int}")]
    [Authorize(Policy = "CategoryOwnerPolicy")]
    public async Task<IActionResult> Delete(int id)
    {
        var response = await _awardCategoryService.DeleteAwardCategoryAsync(id);
        return response.Match<IActionResult>(
            onSuccess: _ =>
            {
                _logger.LogInformation("Deleted AwardCategory with ID {Id}.", id);
                return Ok();
            },
            onError: error =>
            {
                _logger.LogError("Failed to delete AwardCategory with ID {Id}. Error: {Error}", id, error);
                return error.Contains("not found")
                    ? NotFound(new { Error = error })
                    : BadRequest(new { Error = error });
            }
        );
    }
}