using AwardSystemAPI.Application.DTOs;
using AwardSystemAPI.Application.Services;
using AwardSystemAPI.Domain.Enums;
using AwardSystemAPI.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AwardSystemAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AnnouncementController : ControllerBase
{
    private readonly IAnnouncementService _service;
    private readonly ILogger<AnnouncementController> _logger;

    public AnnouncementController(
        IAnnouncementService service,
        ILogger<AnnouncementController> logger)
    {
        _service = service;
        _logger = logger;
    }

    [HttpPost]
    [Authorize(Policy = "SponsorOrAdminPolicy")]
    public async Task<ActionResult<AnnouncementResponseDto>> Create([FromBody] AnnouncementCreateDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var userId = User.GetUserId();
        if (userId == null)
        {
            _logger.LogWarning("User ID claim not found.");
            return Unauthorized("User ID is missing from the token.");
        }
            
        var response = await _service.CreateAnnouncementAsync(dto, userId.Value);
        return response.Match<ActionResult>(
            onSuccess: created => 
            {
                _logger.LogInformation("Created announcement {Id}", created.Id);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            },
            onError: error => BadRequest(new { Error = error })
        );
    }

    [HttpGet]
    [Authorize(Policy = "SponsorOrAdminPolicy")]
    public async Task<ActionResult<IEnumerable<AnnouncementResponseDto>>> GetAll()
    {
        var response = await _service.GetAllAnnouncementsAsync();
        return response.Match<ActionResult>(
            onSuccess: list => Ok(list),
            onError: error =>
            {
                _logger.LogError("Failed to retrieve announcements. Error: {Error}", error);
                return BadRequest(new { Error = error });
            });
    }

    // Sponsors’ own MobileUsers announcements
    [HttpGet("my")]
    [Authorize(Policy = "SponsorOrAdminPolicy")]
    public async Task<ActionResult<IEnumerable<AnnouncementResponseDto>>> GetMine()
    {
        var userId = User.GetUserId();
        if (userId == null)
        {
            _logger.LogWarning("User ID claim not found.");
            return Unauthorized("User ID is missing from the token.");
        }
        
        var response = await _service.GetMyCategoryAnnouncementsAsync(userId.Value);
        return response.Match<ActionResult>(
            onSuccess: list => Ok(list),
            onError: error => BadRequest(new { Error = error })
        );
    }
    
    [HttpGet("by-sponsor/{sponsorId:int}")]
    [Authorize(Policy = "AdminOnlyPolicy")]
    public async Task<ActionResult<IEnumerable<AnnouncementResponseDto>>> GetBySponsor(int sponsorId)
    {
        var response = await _service.GetAnnouncementsByCreatorIdAsync(sponsorId);
        return response.Match<ActionResult>(
            onSuccess: list => Ok(list),
            onError: error => BadRequest(new { Error = error })
        );
    }
    
    [HttpGet("{id:int}")]
    [Authorize(Policy = "SponsorOrAdminPolicy")]
    public async Task<ActionResult<AnnouncementResponseDto>> GetById(int id)
    {
        if (id <= 0)
        {
            _logger.LogWarning("Invalid announcement ID {Id} provided.", id);
            return BadRequest(new { Error = "Invalid ID provided." });
        }

        var response = await _service.GetAnnouncementByIdAsync(id);
        return response.Match<ActionResult>(
            onSuccess: dto => Ok(dto),
            onError: error => error.Contains("not found", StringComparison.OrdinalIgnoreCase)
                ? NotFound(new { Error = error })
                : BadRequest(new { Error = error })
        );
    }

    // Mobile app pulls published announcements 
    [HttpGet("mobile")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<AnnouncementResponseDto>>> GetMobile()
    {
        var response = await _service.GetPublishedForAudienceAsync(TargetAudience.MobileUsers);
        return response.Match<ActionResult>(
            onSuccess: list => Ok(list),
            onError: error => BadRequest(new { Error = error })
        );
    }

    // Sponsors pull published sponsor announcements
    [HttpGet("sponsors")]
    [Authorize(Policy = "SponsorOrAdminPolicy")]
    public async Task<ActionResult<IEnumerable<AnnouncementResponseDto>>> GetSponsor()
    {
        var response = await _service.GetPublishedForAudienceAsync(TargetAudience.Sponsors);
        return response.Match<ActionResult>(
            onSuccess: list => Ok(list),
            onError: error => BadRequest(new { Error = error })
        );
    }

    // Update (sponsor must own or admin)
    [HttpPut("{id:int}")]
    [Authorize(Policy = "CategoryOwnerPolicy")]
    public async Task<IActionResult> Update(int id, [FromBody] AnnouncementUpdateDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var userId = User.GetUserId();
        if (userId == null)
        {
            _logger.LogWarning("User ID claim not found.");
            return Unauthorized("User ID is missing from the token.");
        }

        var response = await _service.UpdateAnnouncementAsync(id, dto, userId.Value);
        return response.Match<IActionResult>(
            onSuccess: _ => Ok(),
            onError: err => err.Contains("not found", StringComparison.OrdinalIgnoreCase)
                ? NotFound(new { Error = err })
                : BadRequest(new { Error = err })
        );
    }

    [HttpDelete("{id:int}")]
    [Authorize(Policy = "CategoryOwnerPolicy")]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = User.GetUserId();
        if (userId == null)
        {
            _logger.LogWarning("User ID claim not found.");
            return Unauthorized("User ID is missing from the token.");
        }
        
        var response = await _service.DeleteAnnouncementAsync(id, userId.Value);
        return response.Match<IActionResult>(
            onSuccess: _ => Ok(),
            onError: error => error.Contains("not found", StringComparison.OrdinalIgnoreCase)
                ? NotFound(new { Error = error })
                : BadRequest(new { Error = error }));
    }
    
    [HttpPost("upload-image")]
    public async Task<IActionResult> UploadImage(IFormFile image, [FromServices] IBlobService blobService)
    {
        if (image == null || image.Length == 0 || !image.ContentType.StartsWith("image/"))
            return BadRequest("Invalid image file.");

        var fileName = $"{Guid.NewGuid()}_{image.FileName}";
        var url = await blobService.UploadAsync(image, fileName);

        return Ok(new { url });
    }

}