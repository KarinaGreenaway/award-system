using System.Security.Claims;
using AwardSystemAPI.Application.DTOs;
using AwardSystemAPI.Application.Services;
using AwardSystemAPI.Common;
using AwardSystemAPI.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace AwardSystemAPI.Tests.UnitTests.NominationTests;

public class NominationControllerTests
{
    private readonly Mock<INominationService> _serviceMock;
    private readonly NominationController _controller;

    public NominationControllerTests()
    {
        Mock<ILogger<NominationController>> loggerMock = new();
        _serviceMock = new Mock<INominationService>();
        _controller = new NominationController(_serviceMock.Object, loggerMock.Object);
    }

    private ClaimsPrincipal CreateUserWithId(string userId)
    {
        var claims = new List<Claim> { new Claim("userId", userId) };
        var identity = new ClaimsIdentity(claims, "FakeScheme");
        return new ClaimsPrincipal(identity);
    }

    [Fact]
    public async Task Create_WhenModelInvalid_ShouldReturnBadRequest()
    {
        _controller.ModelState.AddModelError("dummy", "error");
        var dto = new NominationCreateDto();

        var result = await _controller.Create(dto);
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task Create_WhenUserMissing_ShouldReturnUnauthorized()
    {
        // no user in context
        _controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };
        var dto = new NominationCreateDto();

        var result = await _controller.Create(dto);
        result.Result.Should().BeOfType<UnauthorizedObjectResult>();
    }

    [Fact]
    public async Task Create_WhenServiceSuccess_ShouldReturnCreated()
    {
        var dto = new NominationCreateDto();
        var responseDto = new NominationResponseDto { Id = 123 };
        ApiResponse<NominationResponseDto, string> apiResponse = responseDto;
        _serviceMock.Setup(s => s.CreateNominationAsync(dto, 42)).ReturnsAsync(apiResponse);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = CreateUserWithId("42") }
        };

        var result = await _controller.Create(dto);
        result.Result.Should().BeOfType<CreatedAtActionResult>();
        var created = result.Result as CreatedAtActionResult;
        created?.Value.Should().BeEquivalentTo(responseDto);
        created?.RouteValues?["id"].Should().Be(123);
    }

    [Fact]
    public async Task Create_WhenServiceError_ShouldReturnBadRequest()
    {
        var dto = new NominationCreateDto();
        const string errorMsg = "fail";
        ApiResponse<NominationResponseDto, string> apiResponse = errorMsg;
        _serviceMock.Setup(s => s.CreateNominationAsync(dto, 42)).ReturnsAsync(apiResponse);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = CreateUserWithId("42") }
        };

        var result = await _controller.Create(dto);
        result.Result.Should().BeOfType<BadRequestObjectResult>();
        var bad = result.Result as BadRequestObjectResult;
        bad?.Value.Should().BeEquivalentTo(new { Error = errorMsg });
    }

    [Fact]
    public async Task GetMine_WhenUserMissing_ShouldReturnUnauthorized()
    {
        _controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };

        var result = await _controller.GetMine();
        result.Result.Should().BeOfType<UnauthorizedObjectResult>();
    }

    [Fact]
    public async Task GetMine_WhenServiceSuccess_ShouldReturnOk()
    {
        var list = new List<NominationResponseDto> { new NominationResponseDto() };
        ApiResponse<IEnumerable<NominationResponseDto>, string> apiResponse = list;
        _serviceMock.Setup(s => s.GetNominationsByCreatorIdAsync(42)).ReturnsAsync(apiResponse);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = CreateUserWithId("42") }
        };

        var result = await _controller.GetMine();
        result.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task GetMine_WhenServiceError_ShouldReturnBadRequest()
    {
        const string errorMsg = "error";
        ApiResponse<IEnumerable<NominationResponseDto>, string> apiResponse = errorMsg;
        _serviceMock.Setup(s => s.GetNominationsByCreatorIdAsync(42)).ReturnsAsync(apiResponse);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = CreateUserWithId("42") }
        };

        var result = await _controller.GetMine();
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task GetForMe_WhenUserMissing_ShouldReturnUnauthorized()
    {
        _controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };

        var result = await _controller.GetForMe();
        result.Result.Should().BeOfType<UnauthorizedObjectResult>();
    }

    [Fact]
    public async Task GetForMe_WhenServiceSuccess_ShouldReturnOk()
    {
        var list = new List<NominationResponseDto> { new NominationResponseDto() };
        ApiResponse<IEnumerable<NominationResponseDto>, string> apiResponse = list;
        _serviceMock.Setup(s => s.GetNominationsForNomineeIdAsync(42)).ReturnsAsync(apiResponse);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = CreateUserWithId("42") }
        };

        var result = await _controller.GetForMe();
        result.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task GetTeam_WhenUserMissing_ShouldReturnUnauthorized()
    {
        _controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };

        var result = await _controller.GetTeam();
        result.Result.Should().BeOfType<UnauthorizedObjectResult>();
    }

    [Fact]
    public async Task GetTeam_WhenServiceSuccess_ShouldReturnOk()
    {
        var list = new List<NominationResponseDto> { new NominationResponseDto() };
        ApiResponse<IEnumerable<NominationResponseDto>, string> apiResponse = list;
        _serviceMock.Setup(s => s.GetTeamNominationsForMemberAsync(42)).ReturnsAsync(apiResponse);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = CreateUserWithId("42") }
        };

        var result = await _controller.GetTeam();
        result.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task GetById_WhenInvalidId_ShouldReturnBadRequest()
    {
        var result = await _controller.GetById(0);
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task GetById_WhenServiceSuccess_ShouldReturnOk()
    {
        var dto = new NominationResponseDto { Id = 5 };
        ApiResponse<NominationResponseDto?, string> apiResponse = dto;
        _serviceMock.Setup(s => s.GetNominationByIdAsync(5)).ReturnsAsync(apiResponse);

        var result = await _controller.GetById(5);
        result.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task GetById_WhenServiceNotFound_ShouldReturnNotFound()
    {
        const string errorMsg = "Nomination with ID 5 not found.";
        ApiResponse<NominationResponseDto?, string> apiResponse = errorMsg;
        _serviceMock.Setup(s => s.GetNominationByIdAsync(5)).ReturnsAsync(apiResponse);

        var result = await _controller.GetById(5);
        result.Result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task GetById_WhenServiceError_ShouldReturnBadRequest()
    {
        const string errorMsg = "other error";
        ApiResponse<NominationResponseDto?, string> apiResponse = errorMsg;
        _serviceMock.Setup(s => s.GetNominationByIdAsync(5)).ReturnsAsync(apiResponse);

        var result = await _controller.GetById(5);
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }
}