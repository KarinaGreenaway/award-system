using AwardSystemAPI.Application.DTOs;
using AwardSystemAPI.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AwardSystemAPI.Extensions;

namespace AwardSystemAPI.Controllers;

[ApiController]
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
    public async Task<IActionResult> GetAll()
    {
        var categories = await _awardCategoryService.GetAllAwardCategoriesAsync();
        return Ok(categories);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var category = await _awardCategoryService.GetAwardCategoryByIdAsync(id);
        if (category == null)
        {
            _logger.LogWarning("AwardCategory with ID {Id} not found.", id);
            return NotFound();
        }
        return Ok(category);
    }

    // Returns categories for the currently authenticated user.
    [HttpGet("my")]
    [Authorize(Roles = "Sponsor")]
    public async Task<IActionResult> GetMyCategories()
    {
        int? userId = User.GetUserId();
        if (userId == null)
        {
            _logger.LogWarning("User ID claim not found.");
            return Unauthorized("User ID is missing from the token.");
        }
            
        var categories = await _awardCategoryService.GetAwardCategoriesBySponsorIdAsync(userId.Value);
        return Ok(categories);
    }

    [HttpPost]
    [Authorize(Roles = "Sponsor")]
    public async Task<IActionResult> Create([FromBody] AwardCategoryCreateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var createdCategory = await _awardCategoryService.CreateAwardCategoryAsync(dto);
        _logger.LogInformation("Created AwardCategory with ID {Id}.", createdCategory.Id);
        return CreatedAtAction(nameof(GetById), new { id = createdCategory.Id }, createdCategory);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Sponsor")]
    public async Task<IActionResult> Update(int id, [FromBody] AwardCategoryUpdateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        int? userId = User.GetUserId();
        if (userId == null)
        {
            _logger.LogWarning("User ID claim not found.");
            return Unauthorized("User ID is missing from the token.");
        }

        var existingCategory = await _awardCategoryService.GetAwardCategoryByIdAsync(id);
        if (existingCategory == null)
        {
            _logger.LogWarning("AwardCategory with ID {Id} not found for update.", id);
            return NotFound();
        }
        if (existingCategory.SponsorId != userId.Value)
        {
            _logger.LogWarning("User with ID {UserId} attempted to update AwardCategory with ID {Id} not owned by them.", userId, id);
            return Forbid();
        }

        var updated = await _awardCategoryService.UpdateAwardCategoryAsync(id, dto);
        if (!updated)
        {
            _logger.LogWarning("AwardCategory with ID {Id} not found for update.", id);
            return NotFound();
        }
        _logger.LogInformation("Updated AwardCategory with ID {Id}.", id);
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Sponsor")]
    public async Task<IActionResult> Delete(int id)
    {
        int? userId = User.GetUserId();
        if (userId == null)
        {
            _logger.LogWarning("User ID claim not found.");
            return Unauthorized("User ID is missing from the token.");
        }

        var existingCategory = await _awardCategoryService.GetAwardCategoryByIdAsync(id);
        if (existingCategory == null)
        {
            _logger.LogWarning("AwardCategory with ID {Id} not found for deletion.", id);
            return NotFound();
        }
        if (existingCategory.SponsorId != userId.Value)
        {
            _logger.LogWarning("User with ID {UserId} attempted to delete AwardCategory with ID {Id} not owned by them.", userId, id);
            return Forbid();
        }

        var deleted = await _awardCategoryService.DeleteAwardCategoryAsync(id);
        if (!deleted)
        {
            _logger.LogWarning("AwardCategory with ID {Id} not found for deletion.", id);
            return NotFound();
        }
        _logger.LogInformation("Deleted AwardCategory with ID {Id}.", id);
        return NoContent();
    }
}