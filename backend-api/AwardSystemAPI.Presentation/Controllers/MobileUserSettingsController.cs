using AwardSystemAPI.Application.DTOs.MobileUserSettingsDtos;
using AwardSystemAPI.Application.Services;
using AwardSystemAPI.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AwardSystemAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class MobileUserSettingsController : ControllerBase
    {
        private readonly IMobileUserSettingsService _settingsService;
        private readonly ILogger<MobileUserSettingsController> _logger;

        public MobileUserSettingsController(IMobileUserSettingsService settingsService, ILogger<MobileUserSettingsController> logger)
        {
            _settingsService = settingsService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<MobileUserSettingsResponseDto>> GetSettings()
        {
            var userId = User.GetUserId();
            if (userId == null)
            {
                _logger.LogWarning("User ID claim not found.");
                return Unauthorized(new { Error = "User ID is missing from the token." });
            }

            var response = await _settingsService.GetSettingsAsync(userId.Value);
            return response.Match<ActionResult>(
                onSuccess: result => Ok(result),
                onError: error =>
                {
                    _logger.LogError("Error retrieving settings for user {UserId}: {Error}", userId, error);
                    return BadRequest(new { Error = error });
                }
            );
        }

        [HttpPut]
        public async Task<ActionResult<MobileUserSettingsResponseDto>> UpdateSettings([FromBody] MobileUserSettingsUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = User.GetUserId();
            if (userId == null)
            {
                _logger.LogWarning("User ID claim not found.");
                return Unauthorized(new { Error = "User ID is missing from the token." });
            }

            var response = await _settingsService.UpdateSettingsAsync(userId.Value, dto);
            return response.Match<ActionResult>(
                onSuccess: updated => Ok(updated),
                onError: error =>
                {
                    _logger.LogError("Error updating settings for user {UserId}: {Error}", userId, error);
                    return BadRequest(new { Error = error });
                }
            );
        }
    }
}
