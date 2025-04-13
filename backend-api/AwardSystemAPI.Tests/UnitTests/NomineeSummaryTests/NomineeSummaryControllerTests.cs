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

namespace AwardSystemAPI.Tests.UnitTests.NomineeSummaryTests;

public class NomineeSummaryControllerTests
{
    private readonly Mock<INomineeSummaryService> _serviceMock;
    private readonly NomineeSummaryController _controller;

    public NomineeSummaryControllerTests()
    {
        Mock<ILogger<NomineeSummaryController>> loggerMock = new();
        _serviceMock = new Mock<INomineeSummaryService>();
        _controller = new NomineeSummaryController(_serviceMock.Object, loggerMock.Object);
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
        var dto = new NomineeSummaryCreateDto();

        var result = await _controller.Create(dto);
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task Create_WhenUserMissing_ShouldReturnUnauthorized()
    {
        _controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };
        var dto = new NomineeSummaryCreateDto();

        var result = await _controller.Create(dto);
        result.Result.Should().BeOfType<UnauthorizedObjectResult>();
    }

    [Fact]
    public async Task Create_WhenServiceSuccess_ShouldReturnCreated()
    {
        var dto = new NomineeSummaryCreateDto { NomineeId = 1, CategoryId = 2, TotalNominations = 3 };
        var responseDto = new NomineeSummaryResponseDto { Id = 5, NomineeId = 1, CategoryId = 2, TotalNominations = 3 };
        ApiResponse<NomineeSummaryResponseDto, string> apiResponse = responseDto;
        _serviceMock.Setup(s => s.CreateNomineeSummaryAsync(dto)).ReturnsAsync(apiResponse);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = CreateUserWithId("42") }
        };

        var result = await _controller.Create(dto);
        result.Result.Should().BeOfType<CreatedAtActionResult>();
        var created = result.Result as CreatedAtActionResult;
        created?.Value.Should().BeEquivalentTo(responseDto);
        created?.RouteValues?["id"].Should().Be(5);
    }

    [Fact]
    public async Task Create_WhenServiceError_ShouldReturnBadRequest()
    {
        var dto = new NomineeSummaryCreateDto { NomineeId = 1, CategoryId = 2, TotalNominations = 3 };
        const string errorMsg = "error";
        ApiResponse<NomineeSummaryResponseDto, string> apiResponse = errorMsg;
        _serviceMock.Setup(s => s.CreateNomineeSummaryAsync(dto)).ReturnsAsync(apiResponse);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = CreateUserWithId("42") }
        };

        var result = await _controller.Create(dto);
        result.Result.Should().BeOfType<BadRequestObjectResult>();
        var objectResult = result.Result as BadRequestObjectResult;
        objectResult?.Value.Should().BeEquivalentTo(new { Error = errorMsg });
    }

    [Fact]
    public async Task Update_WhenModelInvalid_ShouldReturnBadRequest()
    {
        _controller.ModelState.AddModelError("dummy", "error");
        var dto = new NomineeSummaryUpdateDto();

        var result = await _controller.Update(1, dto);
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task Update_WhenServiceSuccess_ShouldReturnOk()
    {
        var dto = new NomineeSummaryUpdateDto { TotalNominations = 2 };
        ApiResponse<bool, string> apiResponse = true;
        _serviceMock.Setup(s => s.UpdateNomineeSummaryAsync(3, dto)).ReturnsAsync(apiResponse);

        var result = await _controller.Update(3, dto);
        result.Should().BeOfType<OkResult>();
    }

    [Fact]
    public async Task Update_WhenServiceNotFound_ShouldReturnNotFound()
    {
        var dto = new NomineeSummaryUpdateDto { TotalNominations = 2 };
        const string errorMsg = "NomineeSummary with ID 3 not found.";
        ApiResponse<bool, string> apiResponse = errorMsg;
        _serviceMock.Setup(s => s.UpdateNomineeSummaryAsync(3, dto)).ReturnsAsync(apiResponse);

        var result = await _controller.Update(3, dto);
        result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task Update_WhenServiceError_ShouldReturnBadRequest()
    {
        var dto = new NomineeSummaryUpdateDto { TotalNominations = 2 };
        const string errorMsg = "other error";
        ApiResponse<bool, string> apiResponse = errorMsg;
        _serviceMock.Setup(s => s.UpdateNomineeSummaryAsync(3, dto)).ReturnsAsync(apiResponse);

        var result = await _controller.Update(3, dto);
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task GetAll_WhenServiceSuccess_ShouldReturnOk()
    {
        var list = new List<NomineeSummaryResponseDto> { new NomineeSummaryResponseDto() };
        ApiResponse<IEnumerable<NomineeSummaryResponseDto>, string> apiResponse = list;
        _serviceMock.Setup(s => s.GetAllNomineeSummariesAsync()).ReturnsAsync(apiResponse);

        var result = await _controller.GetAll();
        result.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task GetAll_WhenServiceError_ShouldReturnBadRequest()
    {
        const string errorMsg = "error";
        ApiResponse<IEnumerable<NomineeSummaryResponseDto>, string> apiResponse = errorMsg;
        _serviceMock.Setup(s => s.GetAllNomineeSummariesAsync()).ReturnsAsync(apiResponse);

        var result = await _controller.GetAll();
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task GetByCategoryId_WhenServiceSuccess_ShouldReturnOk()
    {
        var list = new List<NomineeSummaryResponseDto> { new NomineeSummaryResponseDto() };
        ApiResponse<IEnumerable<NomineeSummaryResponseDto>, string> apiResponse = list;
        _serviceMock.Setup(s => s.GetAllNomineeSummariesByCategoryIdAsync(4)).ReturnsAsync(apiResponse);

        var result = await _controller.GetByCategoryId(4);
        result.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task GetByCategoryId_WhenServiceError_ShouldReturnBadRequest()
    {
        const string errorMsg = "error";
        ApiResponse<IEnumerable<NomineeSummaryResponseDto>, string> apiResponse = errorMsg;
        _serviceMock.Setup(s => s.GetAllNomineeSummariesByCategoryIdAsync(4)).ReturnsAsync(apiResponse);

        var result = await _controller.GetByCategoryId(4);
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }
}