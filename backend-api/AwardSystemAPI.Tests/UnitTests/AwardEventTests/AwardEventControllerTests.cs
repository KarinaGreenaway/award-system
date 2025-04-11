using AwardSystemAPI.Application.DTOs;
using AwardSystemAPI.Application.Services;
using AwardSystemAPI.Common;
using AwardSystemAPI.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace AwardSystemAPI.Tests.UnitTests.AwardEventTests;

public class AwardEventControllerTests
{
    private readonly Mock<IAwardEventService> _serviceMock;
    private readonly Mock<ILogger<AwardEventController>> _loggerMock;
    private readonly AwardEventController _controller;

    public AwardEventControllerTests()
    {
        _serviceMock = new Mock<IAwardEventService>();
        _loggerMock = new Mock<ILogger<AwardEventController>>();
        _controller = new AwardEventController(_serviceMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task GetAll_WhenServiceReturnsSuccess_ShouldReturnOkWithDtos()
    {
        var dtos = new List<AwardEventResponseDto>
        {
            new AwardEventResponseDto { Id = 1, Name = "Event 1", Location = "Location 1", EventDateTime = DateTime.UtcNow.AddDays(2), Description = "Desc 1", Directions = "Directions 1", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new AwardEventResponseDto { Id = 2, Name = "Event 2", Location = "Location 2", EventDateTime = DateTime.UtcNow.AddDays(3), Description = "Desc 2", Directions = "Directions 2", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
        };
        ApiResponse<IEnumerable<AwardEventResponseDto>, string> apiResponse = dtos;
        _serviceMock.Setup(s => s.GetAllAwardEventsAsync()).ReturnsAsync(apiResponse);

        ActionResult<IEnumerable<AwardEventResponseDto>> result = await _controller.GetAll();

        result.Result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeEquivalentTo(dtos);
    }


    [Fact]
    public async Task GetById_WithInvalidId_ShouldReturnBadRequest()
    {
        const int invalidId = 0;

        ActionResult<AwardEventResponseDto> result = await _controller.GetById(invalidId);

        result.Result.Should().BeOfType<BadRequestObjectResult>()
            .Which.Value.Should().BeEquivalentTo(new { Error = "Invalid AwardEvent ID provided." });
    }

    [Fact]
    public async Task GetById_WhenEventExists_ShouldReturnOkWithDto()
    {
        const int id = 1;
        var dto = new AwardEventResponseDto
        {
            Id = id,
            Name = "Event 1",
            Location = "Location 1",
            EventDateTime = DateTime.UtcNow.AddDays(2),
            Description = "Desc 1",
            Directions = "Directions 1",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        ApiResponse<AwardEventResponseDto, string> apiResponse = dto;
        _serviceMock.Setup(s => s.GetAwardEventByIdAsync(id)).ReturnsAsync(apiResponse);

        ActionResult<AwardEventResponseDto> result = await _controller.GetById(id);

        result.Result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeEquivalentTo(dto);
    }

    [Fact]
    public async Task GetById_WhenEventDoesNotExist_ShouldReturnNotFound()
    {
        const int id = 1;
        var errorMsg = $"AwardEvent with ID {id} not found.";
        ApiResponse<AwardEventResponseDto, string> apiResponse = errorMsg;
        _serviceMock.Setup(s => s.GetAwardEventByIdAsync(id)).ReturnsAsync(apiResponse);

        ActionResult<AwardEventResponseDto> result = await _controller.GetById(id);

        result.Result.Should().BeOfType<NotFoundObjectResult>()
            .Which.Value.Should().BeEquivalentTo(new { Error = errorMsg });
    }


    [Fact]
    public async Task Create_WhenModelStateIsInvalid_ShouldReturnBadRequest()
    {
        _controller.ModelState.AddModelError("Name", "Name is required");
        var dto = new AwardEventCreateDto();

        ActionResult<AwardEventResponseDto> result = await _controller.Create(dto);

        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task Create_WhenServiceReturnsSuccess_ShouldReturnCreatedAtAction()
    {
        var dto = new AwardEventCreateDto
        {
            Name = "Event 1",
            Location = "Location 1",
            EventDateTime = DateTime.UtcNow.AddDays(2),
            Description = "Desc 1",
            Directions = "Directions 1"
        };

        var createdDto = new AwardEventResponseDto
        {
            Id = 1,
            Name = dto.Name,
            Location = dto.Location,
            EventDateTime = dto.EventDateTime,
            Description = dto.Description,
            Directions = dto.Directions,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        ApiResponse<AwardEventResponseDto, string> apiResponse = createdDto;
        _serviceMock.Setup(s => s.CreateAwardEventAsync(dto)).ReturnsAsync(apiResponse);

        ActionResult<AwardEventResponseDto> result = await _controller.Create(dto);

        result.Result.Should().BeOfType<CreatedAtActionResult>();
        var createdResult = result.Result as CreatedAtActionResult;
        createdResult?.RouteValues?["id"].Should().Be(createdDto.Id);
        createdResult?.Value.Should().BeEquivalentTo(createdDto);
    }

    [Fact]
    public async Task Create_WhenServiceReturnsError_ShouldReturnBadRequest()
    {
        var dto = new AwardEventCreateDto
        {
            Name = "Event 1",
            Location = "Location 1",
            EventDateTime = DateTime.UtcNow.AddDays(2),
            Description = "Desc 1",
            Directions = "Directions 1"
        };
        const string errorMsg = "Creation failed";
        ApiResponse<AwardEventResponseDto, string> apiResponse = errorMsg;
        _serviceMock.Setup(s => s.CreateAwardEventAsync(dto)).ReturnsAsync(apiResponse);

        ActionResult<AwardEventResponseDto> result = await _controller.Create(dto);

        result.Result.Should().BeOfType<BadRequestObjectResult>();
        var badResult = result.Result as BadRequestObjectResult;
        badResult?.Value.Should().BeEquivalentTo(new { Error = errorMsg });
    }

    [Fact]
    public async Task Update_WhenModelStateIsInvalid_ShouldReturnBadRequest()
    {
        _controller.ModelState.AddModelError("Name", "Required");
        var dto = new AwardEventUpdateDto();
        const int id = 1;

        IActionResult result = await _controller.Update(id, dto);

        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task Update_WhenServiceReturnsSuccess_ShouldReturnOk()
    {
        const int id = 1;
        var dto = new AwardEventUpdateDto
        {
            Name = "Updated Event",
            Location = "Updated Location",
            EventDateTime = DateTime.UtcNow.AddDays(3),
            Description = "Updated Desc",
            Directions = "Updated Directions"
        };

        ApiResponse<bool, string> apiResponse = true;
        _serviceMock.Setup(s => s.UpdateAwardEventAsync(id, dto)).ReturnsAsync(apiResponse);

        IActionResult result = await _controller.Update(id, dto);

        result.Should().Match<IActionResult>(r => r is OkResult || r is OkObjectResult);
    }

    [Fact]
    public async Task Update_WhenServiceReturnsNotFoundError_ShouldReturnNotFound()
    {
        const int id = 1;
        var dto = new AwardEventUpdateDto
        {
            Name = "Updated Event",
            Location = "Updated Location",
            EventDateTime = DateTime.UtcNow.AddDays(3),
            Description = "Updated Desc",
            Directions = "Updated Directions"
        };
        string errorMsg = $"AwardEvent with ID {id} not found for update.";
        ApiResponse<bool, string> apiResponse = errorMsg;
        _serviceMock.Setup(s => s.UpdateAwardEventAsync(id, dto)).ReturnsAsync(apiResponse);

        IActionResult result = await _controller.Update(id, dto);

        result.Should().BeOfType<NotFoundObjectResult>()
            .Which.Value.Should().BeEquivalentTo(new { Error = errorMsg });
    }


    [Fact]
    public async Task Delete_WhenServiceReturnsSuccess_ShouldReturnOk()
    {
        const int id = 1;
        ApiResponse<bool, string> apiResponse = true;
        _serviceMock.Setup(s => s.DeleteAwardEventAsync(id)).ReturnsAsync(apiResponse);

        IActionResult result = await _controller.Delete(id);

        result.Should().Match<IActionResult>(r => r is OkResult || r is OkObjectResult);
    }

    [Fact]
    public async Task Delete_WhenServiceReturnsNotFoundError_ShouldReturnNotFound()
    {
        const int id = 1;
        var errorMsg = $"AwardEvent with ID {id} not found for deletion.";
        ApiResponse<bool, string> apiResponse = errorMsg;
        _serviceMock.Setup(s => s.DeleteAwardEventAsync(id)).ReturnsAsync(apiResponse);

        IActionResult result = await _controller.Delete(id);

        result.Should().BeOfType<NotFoundObjectResult>()
            .Which.Value.Should().BeEquivalentTo(new { Error = errorMsg });
    }
}