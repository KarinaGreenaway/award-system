using AwardSystemAPI.Extensions;
using AwardSystemAPI.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AwardSystemAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DeviceTokenController : ControllerBase
{
    private readonly IDeviceTokenRepository _deviceTokenRepo;

    public DeviceTokenController(IDeviceTokenRepository deviceTokenRepo)
    {
        _deviceTokenRepo = deviceTokenRepo;
    }


    [Authorize]
    [HttpPost("api/push/register")]
    public async Task<IActionResult> RegisterToken([FromBody]string token)
    {
        var userId = User.GetUserId();
        if (userId == null)
        {
            return Unauthorized(new { Error = "User ID not found in the context." });
        }
        
        await _deviceTokenRepo.AddOrUpdateTokenAsync(userId.Value, token);
        return Ok();
    }
    
    [Authorize]
    [HttpDelete("api/push/unregister")]
    public async Task<IActionResult> UnregisterToken([FromBody]string token)
    {
        var userId = User.GetUserId();
        if (userId == null)
        {
            return Unauthorized(new { Error = "User ID not found in the context." });
        }
        
        var deviceToken = await _deviceTokenRepo.GetByUserIdAsync(userId.Value);
        if (deviceToken == null || deviceToken.Token != token)
        {
            return NotFound(new { Error = "Device token not found." });
        }

        await _deviceTokenRepo.DeleteAsync(deviceToken);
        return Ok();
    }

}