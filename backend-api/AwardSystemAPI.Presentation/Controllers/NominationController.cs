using AwardSystemAPI.Application.DTOs;
using AwardSystemAPI.Application.Services;
using AwardSystemAPI.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AwardSystemAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NominationController : ControllerBase
{
    private readonly INominationService _nominationService;
    private readonly ILogger<NominationController> _logger;

    public NominationController(INominationService nominationService, ILogger<NominationController> logger)
    {
        _nominationService = nominationService;
        _logger = logger;
    }

    [HttpPost]
    public async Task<ActionResult<NominationResponseDto>> Create([FromBody] NominationCreateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var userId = User.GetUserId();
        if (userId == null)
        {
            _logger.LogWarning("User ID claim not found.");
            return Unauthorized("User ID is missing from the token.");
        }

        var response = await _nominationService.CreateNominationAsync(dto, userId.Value);
        return response.Match<ActionResult>(
            onSuccess: created =>
            {
                _logger.LogInformation("Created Nomination with ID {Id}.", created.Id);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            },
            onError: error =>
            {
                _logger.LogError("Failed to create Nomination. Error: {Error}", error);
                return BadRequest(new { Error = error });
            }
        );
    }

    [HttpGet("mine")]
    public async Task<ActionResult<IEnumerable<NominationResponseDto>>> GetMine()
    {
        var userId = User.GetUserId();
        if (userId == null)
        {
            _logger.LogWarning("User ID claim not found.");
            return Unauthorized("User ID is missing from the token.");
        }

        var response = await _nominationService.GetNominationsByCreatorIdAsync(userId.Value);
        return response.Match<ActionResult>(
            onSuccess: result => Ok(result),
            onError: error =>
            {
                _logger.LogError("Failed to retrieve user nominations. Error: {Error}", error);
                return BadRequest(new { Error = error });
            }
        );
    }
    
    //get nominations for me 
    [HttpGet("for-me")]
    public async Task<ActionResult<IEnumerable<NominationResponseDto>>> GetForMe()
    {
        var userId = User.GetUserId();
        if (userId == null)
        {
            _logger.LogWarning("User ID claim not found.");
            return Unauthorized("User ID is missing from the token.");
        }

        var response = await _nominationService.GetNominationsForNomineeIdAsync(userId.Value);
        return response.Match<ActionResult>(
            onSuccess: result => Ok(result),
            onError: error =>
            {
                _logger.LogError("Failed to retrieve nominations for me. Error: {Error}", error);
                return BadRequest(new { Error = error });
            }
        );
    }

    [HttpGet("team")]
    public async Task<ActionResult<IEnumerable<NominationResponseDto>>> GetTeam()
    {
        var userId = User.GetUserId();
        if (userId == null)
        {
            _logger.LogWarning("User ID claim not found.");
            return Unauthorized("User ID is missing from the token.");
        }

        var response = await _nominationService.GetTeamNominationsForMemberAsync(userId.Value);
        return response.Match<ActionResult>(
            onSuccess: result => Ok(result),
            onError: error =>
            {
                _logger.LogError("Failed to retrieve team nominations. Error: {Error}", error);
                return BadRequest(new { Error = error });
            }
        );
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<NominationResponseDto>> GetById(int id)
    {
        if (id <= 0)
        {
            _logger.LogWarning("Invalid Nomination ID {Id} provided.", id);
            return BadRequest(new { Error = "Invalid Nomination ID provided." });
        }

        var response = await _nominationService.GetNominationByIdAsync(id);
        return response.Match<ActionResult>(
            onSuccess: result => Ok(result),
            onError: error =>
            {
                _logger.LogError("Failed to retrieve Nomination with ID {Id}. Error: {Error}", id, error);
                return error.ToLower().Contains("not found")
                    ? NotFound(new { Error = error })
                    : BadRequest(new { Error = error });
            }
        );
    }
    [Authorize(Policy = "SponsorOrAdminPolicy")]
    [HttpGet("category/{categoryId:int}")]
    public async Task<ActionResult<IEnumerable<NominationResponseDto>>> GetByCategory(int categoryId)
    {
        if (categoryId <= 0)
        {
            _logger.LogWarning("Invalid Category ID {CategoryId} provided.", categoryId);
            return BadRequest(new { Error = "Invalid Category ID provided." });
        }

        var response = await _nominationService.GetNominationsByCategoryIdAsync(categoryId);
        return response.Match<ActionResult>(
            onSuccess: result => Ok(result),
            onError: error =>
            {
                _logger.LogError("Failed to retrieve nominations for Category ID {CategoryId}. Error: {Error}", categoryId, error);
                return error.ToLower().Contains("not found")
                    ? NotFound(new { Error = error })
                    : BadRequest(new { Error = error });
            }
        );
    }
    
    [Authorize(Policy = "SponsorOrAdminPolicy")]
    [HttpGet("nominee/{nomineeId:int}")]
    public async Task<ActionResult<IEnumerable<NominationResponseDto>>> GetByNomineeId(int nomineeId)
    {
        if (nomineeId <= 0)
        {
            _logger.LogWarning("Invalid Nominee ID {NomineeId} provided.", nomineeId);
            return BadRequest(new { Error = "Invalid Nominee ID provided." });
        }

        var response = await _nominationService.GetNominationsForNomineeIdAsync(nomineeId);
        return response.Match<ActionResult>(
            onSuccess: result => Ok(result),
            onError: error =>
            {
                _logger.LogError("Failed to retrieve nominations for Nominee ID {NomineeId}. Error: {Error}", nomineeId, error);
                return error.ToLower().Contains("not found")
                    ? NotFound(new { Error = error })
                    : BadRequest(new { Error = error });
            }
        );
    }
    
}