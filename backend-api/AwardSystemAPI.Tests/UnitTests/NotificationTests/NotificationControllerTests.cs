using System.Security.Claims;
using AwardSystemAPI.Application.DTOs.NotificationDtos;
using AwardSystemAPI.Application.Services;
using AwardSystemAPI.Common;
using AwardSystemAPI.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace AwardSystemAPI.Tests.UnitTests.NotificationTests;

public class NotificationControllerTests
{
    private readonly Mock<INotificationService> _serviceMock;
    private readonly Mock<ILogger<NotificationController>> _loggerMock;
    private readonly NotificationController _controller;

    public NotificationControllerTests()
    {
        _serviceMock = new Mock<INotificationService>();
        _loggerMock = new Mock<ILogger<NotificationController>>();
        _controller = new NotificationController(_serviceMock.Object, _loggerMock.Object);
    }

    // Helper creates a fake ClaimsPrincipal with a "userId" claim set to the provided value.
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
    public async Task GetMyNotifications_WhenUserIdIsMissing_ShouldReturnUnauthorized()
    {
        // context with no user claims.
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext() // no user attached
        };

        ActionResult<IEnumerable<NotificationResponseDto>> result = await _controller.GetMyNotifications();

        result.Result.Should().BeOfType<UnauthorizedObjectResult>()
            .Which.Value.Should().BeEquivalentTo(new { Error = "User ID is missing from the token." });
    }

    [Fact]
    public async Task GetMyNotifications_WhenServiceReturnsSuccess_ShouldReturnOkWithDtos()
    {
        const string userId = "1";
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = CreateUserWithId(userId) }
        };

        var dtos = new List<NotificationResponseDto>
        {
            new NotificationResponseDto { Id = 1, UserId = 1, Title = "Title1", Description = "Desc1", Read = false, CreatedAt = DateTime.UtcNow },
            new NotificationResponseDto { Id = 2, UserId = 1, Title = "Title2", Description = "Desc2", Read = true, CreatedAt = DateTime.UtcNow }
        };
        ApiResponse<IEnumerable<NotificationResponseDto>, string> apiResponse = dtos;
        _serviceMock.Setup(s => s.GetNotificationsAsync(1)).ReturnsAsync(apiResponse);

        var result = await _controller.GetMyNotifications();

        result.Result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeEquivalentTo(dtos);
    }

    [Fact]
    public async Task GetMyNotifications_WhenServiceReturnsError_ShouldReturnBadRequest()
    {
        const string userId = "1";
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = CreateUserWithId(userId) }
        };

        const string errorMsg = "Error retrieving notifications";
        ApiResponse<IEnumerable<NotificationResponseDto>, string> apiResponse = errorMsg;
        _serviceMock.Setup(s => s.GetNotificationsAsync(1)).ReturnsAsync(apiResponse);

        var result = await _controller.GetMyNotifications();

        result.Result.Should().BeOfType<BadRequestObjectResult>()
            .Which.Value.Should().BeEquivalentTo(new { Error = errorMsg });
    }


    [Fact]
    public async Task GetById_WithInvalidId_ShouldReturnBadRequest()
    {
        const int invalidId = 0;

        ActionResult<NotificationResponseDto> result = await _controller.GetById(invalidId);

        result.Result.Should().BeOfType<BadRequestObjectResult>()
            .Which.Value.Should().BeEquivalentTo(new { Error = "Invalid notification ID provided." });
    }

    [Fact]
    public async Task GetById_WhenNotificationExists_ShouldReturnOkWithDto()
    {
        const int id = 1;
        var dto = new NotificationResponseDto { Id = id, UserId = 1, Title = "Title1", Description = "Desc", Read = false, CreatedAt = DateTime.UtcNow };
        ApiResponse<NotificationResponseDto?, string> apiResponse = dto;
        _serviceMock.Setup(s => s.GetNotificationByIdAsync(id)).ReturnsAsync(apiResponse);

        ActionResult<NotificationResponseDto> result = await _controller.GetById(id);

        result.Result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeEquivalentTo(dto);
    }

    [Fact]
    public async Task GetById_WhenNotificationDoesNotExist_ShouldReturnNotFound()
    {
        const int id = 1;
        var errorMsg = $"Notification with ID {id} not found.";
        ApiResponse<NotificationResponseDto?, string> apiResponse = errorMsg;
        _serviceMock.Setup(s => s.GetNotificationByIdAsync(id)).ReturnsAsync(apiResponse);

        ActionResult<NotificationResponseDto> result = await _controller.GetById(id);

        result.Result.Should().BeOfType<NotFoundObjectResult>()
            .Which.Value.Should().BeEquivalentTo(new { Error = errorMsg });
    }

    [Fact]
    public async Task MarkAsRead_WithInvalidId_ShouldReturnBadRequest()
    {
        const int invalidId = 0;

        IActionResult result = await _controller.MarkAsRead(invalidId);

        result.Should().BeOfType<BadRequestObjectResult>()
            .Which.Value.Should().BeEquivalentTo(new { Error = "Invalid notification ID provided." });
    }

    [Fact]
    public async Task MarkAsRead_WhenServiceReturnsSuccess_ShouldReturnOk()
    {
        const int id = 1;
        ApiResponse<bool, string> apiResponse = true;
        _serviceMock.Setup(s => s.MarkAsReadAsync(id)).ReturnsAsync(apiResponse);

        IActionResult result = await _controller.MarkAsRead(id);

        result.Should().BeOfType<OkResult>();
    }

    [Fact]
    public async Task MarkAsRead_WhenServiceReturnsError_ShouldReturnNotFoundOrBadRequest()
    {
        const int id = 1;
        var errorMsg = $"Notification with ID {id} not found.";
        ApiResponse<bool, string> apiResponse = errorMsg;
        _serviceMock.Setup(s => s.MarkAsReadAsync(id)).ReturnsAsync(apiResponse);

        IActionResult result = await _controller.MarkAsRead(id);

        // If error message contains "not found", expect NotFoundObjectResult; otherwise, BadRequestObjectResult.
        if (errorMsg.Contains("not found"))
        {
            result.Should().BeOfType<NotFoundObjectResult>()
                .Which.Value.Should().BeEquivalentTo(new { Error = errorMsg });
        }
        else
        {
            result.Should().BeOfType<BadRequestObjectResult>()
                .Which.Value.Should().BeEquivalentTo(new { Error = errorMsg });
        }
    }

    [Fact]
    public async Task Create_WhenModelStateIsInvalid_ShouldReturnBadRequest()
    {
        _controller.ModelState.AddModelError("Title", "Title is required");
        var dto = new NotificationCreateDto { Title = "", Description = "Desc" };

        ActionResult<NotificationResponseDto> result = await _controller.Create(dto);

        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task Create_WhenUserIdClaimMissing_ShouldReturnUnauthorized()
    {
        // Controller without user claim.
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext() // no claims
        };
        var dto = new NotificationCreateDto { Title = "Title", Description = "Desc" };

        ActionResult<NotificationResponseDto> result = await _controller.Create(dto);

        result.Result.Should().BeOfType<UnauthorizedObjectResult>()
            .Which.Value.Should().BeEquivalentTo(new { Error = "User ID is missing from the token." });
    }

    [Fact]
    public async Task Create_WhenServiceReturnsSuccess_ShouldReturnCreatedAtAction()
    {
        // Simulate a user with userId = 1.
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = CreateUserWithId("1") }
        };

        var inputDto = new NotificationCreateDto { Title = "Title", Description = "Desc" };
        var createdDto = new NotificationResponseDto { Id = 1, UserId = 1, Title = "Title", Description = "Desc", Read = false, CreatedAt = DateTime.UtcNow };
        ApiResponse<NotificationResponseDto, string> apiResponse = createdDto;
        _serviceMock.Setup(s => s.CreateNotificationAsync(It.IsAny<NotificationCreateDto>())).ReturnsAsync(apiResponse);

        ActionResult<NotificationResponseDto> result = await _controller.Create(inputDto);

        result.Result.Should().BeOfType<CreatedAtActionResult>();
        var createdResult = result.Result as CreatedAtActionResult;
        createdResult?.RouteValues?["id"].Should().Be(createdDto.Id);
        createdResult?.Value.Should().BeEquivalentTo(createdDto);
    }

    [Fact]
    public async Task Create_WhenServiceReturnsError_ShouldReturnBadRequest()
    {
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = CreateUserWithId("1") }
        };

        var dto = new NotificationCreateDto { Title = "Title", Description = "Desc" };
        const string errorMsg = "Creation failed";
        ApiResponse<NotificationResponseDto, string> apiResponse = errorMsg;
        _serviceMock.Setup(s => s.CreateNotificationAsync(It.IsAny<NotificationCreateDto>())).ReturnsAsync(apiResponse);

        ActionResult<NotificationResponseDto> result = await _controller.Create(dto);

        result.Result.Should().BeOfType<BadRequestObjectResult>();
        var badResult = result.Result as BadRequestObjectResult;
        badResult?.Value.Should().BeEquivalentTo(new { Error = errorMsg });
    }
}