using AwardSystemAPI.Application.Services;
using AwardSystemAPI.Extensions;
using Microsoft.AspNetCore.Mvc;
using AwardSystemAPI.Application.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace AwardSystemAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NominationQuestionController : ControllerBase
{
    private readonly INominationQuestionService _questionService;
    private readonly ILogger<NominationQuestionController> _logger;

    public NominationQuestionController(INominationQuestionService questionService, ILogger<NominationQuestionController> logger)
    {
        _questionService = questionService;
        _logger = logger;
    }

    [HttpGet("category/{categoryId:int}")]
    [Authorize(Policy = "CategoryOwnerPolicy")]
    public async Task<ActionResult<IEnumerable<NominationQuestionResponseDto>>> GetByCategory(int categoryId)
    {
        var response = await _questionService.GetQuestionsByCategoryAsync(categoryId);
        return response.Match<ActionResult>(
            onSuccess: result => Ok(result),
            onError: error =>
            {
                _logger.LogError("Failed to retrieve NominationQuestions for category {CategoryId}. Error: {Error}", categoryId, error);
                return BadRequest(new { Error = error });
            });
    }

    [HttpGet("{id:int}")]
    [Authorize(Policy = "CategoryOwnerPolicy")]
    public async Task<ActionResult<NominationQuestionResponseDto>> GetById(int id)
    {
        var response = await _questionService.GetQuestionByIdAsync(id);
        return response.Match<ActionResult>(
            onSuccess: dto => Ok(dto),
            onError: error =>
            {
                _logger.LogError("Failed to retrieve NominationQuestion with ID {Id}. Error: {Error}", id, error);
                return NotFound(new { Error = error });
            });
    }

    [HttpPost]
    [Authorize(Policy = "CategoryOwnerPolicy")]
    public async Task<ActionResult<NominationQuestionResponseDto>> Create([FromBody] NominationQuestionCreateDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var response = await _questionService.CreateQuestionAsync(dto);
        return response.Match<ActionResult>(
            onSuccess: created =>
            {
                _logger.LogInformation("Created NominationQuestion with ID {Id}.", created.Id);
               return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            },
            onError: error =>
            {
                _logger.LogError("Failed to create NominationQuestion. Error: {Error}", error);
                return BadRequest(new { Error = error });
            });
    }

    [HttpPut("{id:int}")]
    [Authorize(Policy = "CategoryOwnerPolicy")]
    public async Task<IActionResult> Update(int id, [FromBody] NominationQuestionUpdateDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var userId = User.GetUserId();
        if (userId == null) return Unauthorized("User ID is missing.");

        var res = await _questionService.UpdateQuestionAsync(id, dto);
        return res.Match<IActionResult>(
            onSuccess: _ => Ok(),
            onError: error =>
            {
                _logger.LogError("Failed to update NominationQuestion with ID {Id}. Error: {Error}", id, error);
                return error.ToLower().Contains("not found")
                    ? NotFound(new { Error = error })
                    : BadRequest(new { Error = error });
            });
    }

    [HttpDelete("{id:int}")]
    [Authorize(Policy = "CategoryOwnerPolicy")]
    public async Task<IActionResult> Delete(int id)
    {
        var response = await _questionService.DeleteQuestionAsync(id);
        return response.Match<IActionResult>(
            onSuccess: _ => Ok(),
            onError: error =>
            {
                _logger.LogError("Failed to delete NominationQuestion with ID {Id}. Error: {Error}", id, error);
                return error.ToLower().Contains("not found")
                    ? NotFound(new { Error = error })
                    : BadRequest(new { Error = error });
            });
    }
}