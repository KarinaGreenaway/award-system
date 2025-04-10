using AwardSystemAPI.Application.DTOs;
using AwardSystemAPI.Application.Services;
using AwardSystemAPI.Common;
using AwardSystemAPI.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace AwardSystemAPI.Tests.UnitTests.JudgingRoundTests;

public class JudgingRoundControllerTests
{
    private readonly Mock<IJudgingRoundService> _serviceMock;
    private readonly Mock<ILogger<JudgingRoundController>> _loggerMock;
    private readonly JudgingRoundController _controller;

    public JudgingRoundControllerTests()
    {
        _serviceMock = new Mock<IJudgingRoundService>();
        _loggerMock = new Mock<ILogger<JudgingRoundController>>();
        _controller = new JudgingRoundController(_serviceMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task GetById_WithInvalidId_ShouldReturnBadRequest()
    {
        const int invalidId = 0;

        ActionResult<JudgingRoundResponseDto> result = await _controller.GetById(invalidId);

        result.Result.Should().BeOfType<BadRequestObjectResult>()
            .Which.Value.Should().BeEquivalentTo(new { Error = "Invalid JudgingRound ID provided." });
    }

    [Fact]
    public async Task GetById_WhenJudgingRoundExists_ShouldReturnOkWithDto()
    {
        const int id = 1;
        var dto = new JudgingRoundResponseDto
        {
            Id = id,
            AwardProcessId = 1,
            RoundName = "Round 1",
            StartDate = DateTime.UtcNow.AddDays(1),
            Deadline = DateTime.UtcNow.AddDays(2),
            CandidateCount = 3,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        ApiResponse<JudgingRoundResponseDto, string> apiResponse = dto;
        _serviceMock.Setup(s => s.GetJudgingRoundByIdAsync(id)).ReturnsAsync(apiResponse);

        ActionResult<JudgingRoundResponseDto> result = await _controller.GetById(id);

        result.Result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeEquivalentTo(dto);
    }

    [Fact]
    public async Task GetById_WhenJudgingRoundDoesNotExist_ShouldReturnNotFound()
    {
        const int id = 1;
        var errorMsg = $"JudgingRound with ID {id} not found.";
        ApiResponse<JudgingRoundResponseDto, string> apiResponse = errorMsg;
        _serviceMock.Setup(s => s.GetJudgingRoundByIdAsync(id)).ReturnsAsync(apiResponse);

        ActionResult<JudgingRoundResponseDto> result = await _controller.GetById(id);

        result.Result.Should().BeOfType<NotFoundObjectResult>()
            .Which.Value.Should().BeEquivalentTo(new { Error = errorMsg });
    }

    [Fact]
    public async Task GetByAwardProcess_WhenServiceReturnsSuccess_ShouldReturnOkWithDtos()
    {
        const int awardProcessId = 1;
        var dtos = new List<JudgingRoundResponseDto>
        {
            new JudgingRoundResponseDto
            {
                Id = 1,
                AwardProcessId = awardProcessId,
                RoundName = "Round 1",
                StartDate = DateTime.UtcNow.AddDays(1),
                Deadline = DateTime.UtcNow.AddDays(2),
                CandidateCount = 3,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };

        ApiResponse<IEnumerable<JudgingRoundResponseDto>, string> apiResponse = dtos;
        _serviceMock.Setup(s => s.GetJudgingRoundsByAwardProcessIdAsync(awardProcessId)).ReturnsAsync(apiResponse);

        ActionResult<IEnumerable<JudgingRoundResponseDto>> result = await _controller.GetByAwardProcess(awardProcessId);

        result.Result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeEquivalentTo(dtos);
    }

    [Fact]
    public async Task GetByAwardProcess_WhenServiceReturnsError_ShouldReturnBadRequest()
    {
        const int awardProcessId = 1;
        const string errorMsg = "Error retrieving rounds";
        ApiResponse<IEnumerable<JudgingRoundResponseDto>, string> apiResponse = errorMsg;
        _serviceMock.Setup(s => s.GetJudgingRoundsByAwardProcessIdAsync(awardProcessId)).ReturnsAsync(apiResponse);

        ActionResult<IEnumerable<JudgingRoundResponseDto>> result = await _controller.GetByAwardProcess(awardProcessId);

        result.Result.Should().BeOfType<BadRequestObjectResult>()
            .Which.Value.Should().BeEquivalentTo(new { Error = errorMsg });
    }

    [Fact]
    public async Task Create_WhenModelStateIsInvalid_ShouldReturnBadRequest()
    {
        _controller.ModelState.AddModelError("RoundName", "RoundName is required");
        var dto = new JudgingRoundCreateDto();

        ActionResult<JudgingRoundResponseDto> result = await _controller.Create(dto);

        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task Create_WhenServiceReturnsSuccess_ShouldReturnCreatedAtAction()
    {
        var dto = new JudgingRoundCreateDto
        {
            AwardProcessId = 1,
            RoundName = "Round 1",
            StartDate = DateTime.UtcNow.AddDays(1),
            Deadline = DateTime.UtcNow.AddDays(2),
            CandidateCount = 3
        };

        var createdDto = new JudgingRoundResponseDto
        {
            Id = 1,
            AwardProcessId = dto.AwardProcessId,
            RoundName = dto.RoundName,
            StartDate = dto.StartDate,
            Deadline = dto.Deadline,
            CandidateCount = dto.CandidateCount,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        ApiResponse<JudgingRoundResponseDto, string> apiResponse = createdDto;
        _serviceMock.Setup(s => s.CreateJudgingRoundAsync(dto)).ReturnsAsync(apiResponse);

        ActionResult<JudgingRoundResponseDto> result = await _controller.Create(dto);

        result.Result.Should().BeOfType<CreatedAtActionResult>();
        var createdResult = result.Result as CreatedAtActionResult;
        createdResult?.RouteValues?["id"].Should().Be(createdDto.Id);
        createdResult?.Value.Should().BeEquivalentTo(createdDto);
    }

    [Fact]
    public async Task Create_WhenServiceReturnsError_ShouldReturnBadRequest()
    {
        var dto = new JudgingRoundCreateDto
        {
            AwardProcessId = 1,
            RoundName = "Round 1",
            StartDate = DateTime.UtcNow.AddDays(1),
            Deadline = DateTime.UtcNow.AddDays(2),
            CandidateCount = 3
        };
        const string errorMsg = "Creation failed";
        ApiResponse<JudgingRoundResponseDto, string> apiResponse = errorMsg;
        _serviceMock.Setup(s => s.CreateJudgingRoundAsync(dto)).ReturnsAsync(apiResponse);

        ActionResult<JudgingRoundResponseDto> result = await _controller.Create(dto);

        result.Result.Should().BeOfType<BadRequestObjectResult>();
        var badResult = result.Result as BadRequestObjectResult;
        badResult?.Value.Should().BeEquivalentTo(new { Error = errorMsg });
    }


    [Fact]
    public async Task Update_WhenModelStateIsInvalid_ShouldReturnBadRequest()
    {
        _controller.ModelState.AddModelError("RoundName", "Required");
        var dto = new JudgingRoundUpdateDto();
        const int id = 1;

        IActionResult result = await _controller.Update(id, dto);

        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task Update_WhenServiceReturnsSuccess_ShouldReturnOk()
    {
        const int id = 1;
        var dto = new JudgingRoundUpdateDto
        {
            RoundName = "Updated Round",
            StartDate = DateTime.UtcNow.AddDays(3),
            Deadline = DateTime.UtcNow.AddDays(4),
            CandidateCount = 5
        };

        ApiResponse<bool, string> apiResponse = true;
        _serviceMock.Setup(s => s.UpdateJudgingRoundAsync(id, dto)).ReturnsAsync(apiResponse);

        IActionResult result = await _controller.Update(id, dto);

        result.Should().Match<IActionResult>(r => r is OkResult);
    }

    [Fact]
    public async Task Update_WhenServiceReturnsNotFoundError_ShouldReturnNotFound()
    {
        const int id = 1;
        var dto = new JudgingRoundUpdateDto
        {
            RoundName = "Updated Round",
            StartDate = DateTime.UtcNow.AddDays(3),
            Deadline = DateTime.UtcNow.AddDays(4),
            CandidateCount = 5
        };
        var errorMsg = $"JudgingRound with ID {id} not found for update.";
        ApiResponse<bool, string> apiResponse = errorMsg;
        _serviceMock.Setup(s => s.UpdateJudgingRoundAsync(id, dto)).ReturnsAsync(apiResponse);

        IActionResult result = await _controller.Update(id, dto);

        result.Should().BeOfType<NotFoundObjectResult>()
            .Which.Value.Should().BeEquivalentTo(new { Error = errorMsg });
    }


    [Fact]
    public async Task Delete_WhenServiceReturnsSuccess_ShouldReturnOk()
    {
        const int id = 1;
        ApiResponse<bool, string> apiResponse = true;
        _serviceMock.Setup(s => s.DeleteJudgingRoundAsync(id)).ReturnsAsync(apiResponse);

        IActionResult result = await _controller.Delete(id);

        result.Should().Match<IActionResult>(r => r is OkResult || r is OkObjectResult);
    }

    [Fact]
    public async Task Delete_WhenServiceReturnsNotFoundError_ShouldReturnNotFound()
    {
        const int id = 1;
        var errorMsg = $"JudgingRound with ID {id} not found for deletion.";
        ApiResponse<bool, string> apiResponse = errorMsg;
        _serviceMock.Setup(s => s.DeleteJudgingRoundAsync(id)).ReturnsAsync(apiResponse);

        IActionResult result = await _controller.Delete(id);

        result.Should().BeOfType<NotFoundObjectResult>()
            .Which.Value.Should().BeEquivalentTo(new { Error = errorMsg });
    }
}