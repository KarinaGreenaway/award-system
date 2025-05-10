using AwardSystemAPI.Application.DTOs;
using AwardSystemAPI.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AwardSystemAPI.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class UserController: ControllerBase
{
    private readonly IUserService _userService;
    private readonly ILogger<UserController> _logger;

    public UserController(IUserService userService, ILogger<UserController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserResponseDto>>> GetUsers()
    {
        var response = await _userService.GetAllUsersAsync();
        return response.Match<ActionResult>(
            onSuccess: result => Ok(result),
            onError: error =>
            {
                _logger.LogError("Failed to retrieve users. Error: {Error}", error);
                return error.ToLower().Contains("not found")
                    ? NotFound(new { Error = error })
                    : BadRequest(new { Error = error });
            }
        );
    }
}