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

namespace AwardSystemAPI.Tests.UnitTests.NominationQuestionTests;

public class NominationQuestionControllerTests
{
    private readonly Mock<INominationQuestionService> _serviceMock;
    private readonly NominationQuestionController _controller;

    public NominationQuestionControllerTests()
    {
        Mock<ILogger<NominationQuestionController>> loggerMock = new();
        _serviceMock = new Mock<INominationQuestionService>();
        _controller = new NominationQuestionController(_serviceMock.Object, loggerMock.Object);
    }

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
    public async Task GetByCategory_WhenServiceReturnsSuccess_ShouldReturnOk()
    {
        const int categoryId = 3;
        var dtos = new List<NominationQuestionResponseDto>
        {
            new NominationQuestionResponseDto { Id = 1, CategoryId = categoryId, QuestionText = "Q" }
        };
        ApiResponse<IEnumerable<NominationQuestionResponseDto>, string> apiResponse = dtos;
        _serviceMock.Setup(s => s.GetQuestionsByCategoryAsync(categoryId)).ReturnsAsync(apiResponse);

        var result = await _controller.GetByCategory(categoryId);

        result.Result.Should().BeOfType<OkObjectResult>();
        var ok = result.Result as OkObjectResult;
        ok?.Value.Should().BeEquivalentTo(dtos);
    }

    [Fact]
    public async Task GetByCategory_WhenServiceReturnsError_ShouldReturnBadRequest()
    {
        const int categoryId = 3;
        const string errorMsg = "Error fetching";
        ApiResponse<IEnumerable<NominationQuestionResponseDto>, string> apiResponse = errorMsg;
        _serviceMock.Setup(s => s.GetQuestionsByCategoryAsync(categoryId)).ReturnsAsync(apiResponse);

        var result = await _controller.GetByCategory(categoryId);

        result.Result.Should().BeOfType<BadRequestObjectResult>();
        var bad = result.Result as BadRequestObjectResult;
        bad?.Value.Should().BeEquivalentTo(new { Error = errorMsg });
    }

    [Fact]
    public async Task GetById_WhenExists_ShouldReturnOk()
    {
        const int id = 5;
        var dto = new NominationQuestionResponseDto { Id = id, CategoryId = 2, QuestionText = "Q" };
        ApiResponse<NominationQuestionResponseDto?, string> apiResponse = dto;
        _serviceMock.Setup(s => s.GetQuestionByIdAsync(id)).ReturnsAsync(apiResponse);

        var result = await _controller.GetById(id);

        result.Result.Should().BeOfType<OkObjectResult>();
        var ok = result.Result as OkObjectResult;
        ok?.Value.Should().BeEquivalentTo(dto);
    }

    [Fact]
    public async Task GetById_WhenNotExists_ShouldReturnNotFound()
    {
        const int id = 5;
        var errorMsg = $"NominationQuestion with ID {id} not found.";
        ApiResponse<NominationQuestionResponseDto?, string> apiResponse = errorMsg;
        _serviceMock.Setup(s => s.GetQuestionByIdAsync(id)).ReturnsAsync(apiResponse);

        var result = await _controller.GetById(id);

        result.Result.Should().BeOfType<NotFoundObjectResult>();
        var foundObjectResult = result.Result as NotFoundObjectResult;
        foundObjectResult?.Value.Should().BeEquivalentTo(new { Error = errorMsg });
    }

    [Fact]
    public async Task Create_WhenModelInvalid_ShouldReturnBadRequest()
    {
        _controller.ModelState.AddModelError("QuestionText", "Required");
        var dto = new NominationQuestionCreateDto();

        var result = await _controller.Create(dto);

        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }
        
    [Fact]
    public async Task Create_WhenServiceSuccess_ShouldReturnCreated()
    {
        _controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = CreateUserWithId("42") } };
        var dto = new NominationQuestionCreateDto { CategoryId = 1, QuestionText = "Q" };
        var responseDto = new NominationQuestionResponseDto { Id = 10, CategoryId = 1, QuestionText = "Q" };
        ApiResponse<NominationQuestionResponseDto, string> apiResponse = responseDto;
        _serviceMock.Setup(s => s.CreateQuestionAsync(dto)).ReturnsAsync(apiResponse);

        var result = await _controller.Create(dto);

        result.Result.Should().BeOfType<CreatedAtActionResult>();
        var created = result.Result as CreatedAtActionResult;
        created?.Value.Should().BeEquivalentTo(responseDto);
        created?.RouteValues?["id"].Should().Be(10);
    }

    [Fact]
    public async Task Create_WhenServiceError_ShouldReturnBadRequest()
    {
        _controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = CreateUserWithId("42") } };
        var dto = new NominationQuestionCreateDto { CategoryId = 1, QuestionText = "Q" };
        const string errorMsg = "Cannot create";
        ApiResponse<NominationQuestionResponseDto, string> apiResponse = errorMsg;
        _serviceMock.Setup(s => s.CreateQuestionAsync(dto)).ReturnsAsync(apiResponse);

        var result = await _controller.Create(dto);

        result.Result.Should().BeOfType<BadRequestObjectResult>();
        var bad = result.Result as BadRequestObjectResult;
        bad?.Value.Should().BeEquivalentTo(new { Error = errorMsg });
    }

    [Fact]
    public async Task Update_WhenModelInvalid_ShouldReturnBadRequest()
    {
        _controller.ModelState.AddModelError("QuestionText", "Required");
        var dto = new NominationQuestionUpdateDto();

        var result = await _controller.Update(1, dto);

        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task Update_WhenServiceSuccess_ShouldReturnOk()
    {
        var dto = new NominationQuestionUpdateDto { QuestionText = "Updated" };
        ApiResponse<bool, string> apiResponse = true;
        _serviceMock.Setup(s => s.UpdateQuestionAsync(1, dto)).ReturnsAsync(apiResponse);
        _controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = CreateUserWithId("42") } };

        var result = await _controller.Update(1, dto);

        result.Should().BeOfType<OkResult>();
    }

    [Fact]
    public async Task Update_WhenServiceErrorNotFound_ShouldReturnNotFound()
    {
        var dto = new NominationQuestionUpdateDto { QuestionText = "Updated" };
        const string errorMsg = "Not found";
        ApiResponse<bool, string> apiResponse = errorMsg;
        _serviceMock.Setup(s => s.UpdateQuestionAsync(1, dto)).ReturnsAsync(apiResponse);
        _controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = CreateUserWithId("42") } };

        var result = await _controller.Update(1, dto);

        result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task Delete_WhenServiceSuccess_ShouldReturnOk()
    {
        ApiResponse<bool, string> apiResponse = true;
        _serviceMock.Setup(s => s.DeleteQuestionAsync(1)).ReturnsAsync(apiResponse);
        _controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = CreateUserWithId("42") } };

        var result = await _controller.Delete(1);

        result.Should().BeOfType<OkResult>();
    }

    [Fact]
    public async Task Delete_WhenServiceErrorNotFound_ShouldReturnNotFound()
    {
        const string errorMsg = "Not found";
        ApiResponse<bool, string> apiResponse = errorMsg;
        _serviceMock.Setup(s => s.DeleteQuestionAsync(1)).ReturnsAsync(apiResponse);
        _controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = CreateUserWithId("42") } };

        var result = await _controller.Delete(1);

        result.Should().BeOfType<NotFoundObjectResult>();
    }
}