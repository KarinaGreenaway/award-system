using System.Security.Claims;
using AwardSystemAPI.Application.DTOs.MobileUserSettingsDtos;
using AwardSystemAPI.Application.Services;
using AwardSystemAPI.Common;
using AwardSystemAPI.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace AwardSystemAPI.Tests.UnitTests.MobileUserSettingsTests;

public class MobileUserSettingsControllerTests
{
    private readonly Mock<IMobileUserSettingsService> _serviceMock;
    private readonly Mock<ILogger<MobileUserSettingsController>> _loggerMock;
    private readonly MobileUserSettingsController _controller;

    public MobileUserSettingsControllerTests()
    {
        _serviceMock = new Mock<IMobileUserSettingsService>();
        _loggerMock = new Mock<ILogger<MobileUserSettingsController>>();
        _controller = new MobileUserSettingsController(_serviceMock.Object, _loggerMock.Object);
    }

    // Helper to create a fake ClaimsPrincipal with a "userId" claim.
    private ClaimsPrincipal CreateUserWithId(string userId)
    {
        var claims = new List<Claim>
        {
            new Claim("userId", userId)
        };
        var identity = new ClaimsIdentity(claims, "FakeScheme");
        return new ClaimsPrincipal(identity);
    }


    [Fact]
    public async Task GetSettings_WhenUserIdClaimMissing_ShouldReturnUnauthorized()
    {
        // no user claim assigned.
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext() // no user
        };

        ActionResult<MobileUserSettingsResponseDto> result = await _controller.GetSettings();

        result.Result.Should().BeOfType<UnauthorizedObjectResult>()
            .Which.Value.Should().BeEquivalentTo(new { Error = "User ID is missing from the token." });
    }

    [Fact]
    public async Task GetSettings_WhenServiceReturnsSuccess_ShouldReturnOkWithDto()
    {
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = CreateUserWithId("1") }
        };

        var settingsDto = new MobileUserSettingsResponseDto
        {
            Id = 5,
            UserId = 1,
            PushNotifications = false,
            AiFunctionality = true
        };
        ApiResponse<MobileUserSettingsResponseDto, string> apiResponse = settingsDto;
        _serviceMock.Setup(s => s.GetSettingsAsync(1)).ReturnsAsync(apiResponse);

        ActionResult<MobileUserSettingsResponseDto> result = await _controller.GetSettings();

        result.Result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeEquivalentTo(settingsDto);
    }

    [Fact]
    public async Task GetSettings_WhenServiceReturnsError_ShouldReturnBadRequest()
    {
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = CreateUserWithId("1") }
        };

        const string errorMsg = "Settings not found for user with ID 1.";
        ApiResponse<MobileUserSettingsResponseDto, string> apiResponse = errorMsg;
        _serviceMock.Setup(s => s.GetSettingsAsync(1)).ReturnsAsync(apiResponse);

        ActionResult<MobileUserSettingsResponseDto> result = await _controller.GetSettings();

        result.Result.Should().BeOfType<BadRequestObjectResult>()
            .Which.Value.Should().BeEquivalentTo(new { Error = errorMsg });
    }


    [Fact]
    public async Task UpdateSettings_WhenModelStateIsInvalid_ShouldReturnBadRequest()
    {
        // Add a model error.
        _controller.ModelState.AddModelError("PushNotifications", "Required");
        var dto = new MobileUserSettingsUpdateDto { PushNotifications = false, AiFunctionality = true };

        ActionResult<MobileUserSettingsResponseDto> result = await _controller.UpdateSettings(dto);

        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task UpdateSettings_WhenUserIdClaimMissing_ShouldReturnUnauthorized()
    {
        _controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };
        var dto = new MobileUserSettingsUpdateDto { PushNotifications = false, AiFunctionality = true };

        ActionResult<MobileUserSettingsResponseDto> result = await _controller.UpdateSettings(dto);

        result.Result.Should().BeOfType<UnauthorizedObjectResult>()
            .Which.Value.Should().BeEquivalentTo(new { Error = "User ID is missing from the token." });
    }

    [Fact]
    public async Task UpdateSettings_WhenServiceReturnsSuccess_ShouldReturnOkWithDto()
    {
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = CreateUserWithId("1") }
        };

        var updateDto = new MobileUserSettingsUpdateDto { PushNotifications = false, AiFunctionality = true };
        var updatedDto = new MobileUserSettingsResponseDto { Id = 7, UserId = 1, PushNotifications = false, AiFunctionality = true };
        ApiResponse<MobileUserSettingsResponseDto, string> apiResponse = updatedDto;
        _serviceMock.Setup(s => s.UpdateSettingsAsync(1, updateDto)).ReturnsAsync(apiResponse);

        ActionResult<MobileUserSettingsResponseDto> result = await _controller.UpdateSettings(updateDto);

        result.Result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeEquivalentTo(updatedDto);
    }

    [Fact]
    public async Task UpdateSettings_WhenServiceReturnsError_ShouldReturnBadRequest()
    {
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = CreateUserWithId("1") }
        };

        var updateDto = new MobileUserSettingsUpdateDto { PushNotifications = false, AiFunctionality = true };
        const string errorMsg = "Settings not found for user with ID 1.";
        ApiResponse<MobileUserSettingsResponseDto, string> apiResponse = errorMsg;
        _serviceMock.Setup(s => s.UpdateSettingsAsync(1, updateDto)).ReturnsAsync(apiResponse);

        ActionResult<MobileUserSettingsResponseDto> result = await _controller.UpdateSettings(updateDto);

        result.Result.Should().BeOfType<BadRequestObjectResult>()
            .Which.Value.Should().BeEquivalentTo(new { Error = errorMsg });
    }
}