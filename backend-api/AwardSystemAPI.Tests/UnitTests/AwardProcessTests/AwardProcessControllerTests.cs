using AwardSystemAPI.Application.DTOs.AwardProcessDtos;
using AwardSystemAPI.Application.Services;
using AwardSystemAPI.Common;
using AwardSystemAPI.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace AwardSystemAPI.Tests.UnitTests.AwardProcessTests;

public class AwardProcessControllerTests
{
    private readonly Mock<IAwardProcessService> _serviceMock;
    private readonly Mock<ILogger<AwardProcessController>> _loggerMock;
    private readonly AwardProcessController _controller;
        
    public AwardProcessControllerTests()
    {
        _serviceMock = new Mock<IAwardProcessService>();
        _loggerMock = new Mock<ILogger<AwardProcessController>>();
        _controller = new AwardProcessController(_serviceMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task GetAll_WhenServiceReturnsSuccess_ShouldReturn200WithDtoList()
    {
        var dtos = new List<AwardProcessResponseDto>
        {
            new AwardProcessResponseDto { Id = 1, AwardsName = "Award 1", StartDate = DateTime.UtcNow, Status = "active", CreatedAt = DateTime.UtcNow },
            new AwardProcessResponseDto { Id = 2, AwardsName = "Award 2", StartDate = DateTime.UtcNow, Status = "completed", CreatedAt = DateTime.UtcNow }
        };
        ApiResponse<IEnumerable<AwardProcessResponseDto>, string> apiResponse = dtos;
        _serviceMock.Setup(s => s.GetAllAwardProcessesAsync()).ReturnsAsync(apiResponse);
            
        ActionResult<IEnumerable<AwardProcessResponseDto>> actionResult = await _controller.GetAll();
            
        actionResult.Value.Should().BeNull("the controller returns Ok(result) so Value is not set");
        actionResult.Result.Should().BeOfType<OkObjectResult>();
            
        var okResult = actionResult.Result as OkObjectResult;
        okResult?.StatusCode.Should().Be(200);
        okResult?.Value.Should().BeEquivalentTo(dtos, options => options.ComparingByMembers<AwardProcessResponseDto>());
    }

    [Fact]
    public async Task GetAll_WhenServiceReturnsError_ShouldReturn400WithError()
    {
        const string errorMsg = "Service error occurred";
        ApiResponse<IEnumerable<AwardProcessResponseDto>, string> apiResponse = errorMsg;
        _serviceMock.Setup(s => s.GetAllAwardProcessesAsync()).ReturnsAsync(apiResponse);
            
        ActionResult<IEnumerable<AwardProcessResponseDto>> actionResult = await _controller.GetAll();
            
        actionResult.Value.Should().BeNull();
        actionResult.Result.Should().BeOfType<BadRequestObjectResult>();
            
        var badResult = actionResult.Result as BadRequestObjectResult;
        badResult?.StatusCode.Should().Be(400);
        badResult?.Value.Should().BeEquivalentTo(new { Error = errorMsg });
    }

        

    [Fact]
    public async Task GetById_WhenIdIsInvalid_ShouldReturn400()
    {
        const int invalidId = 0;
            
        ActionResult<AwardProcessResponseDto> actionResult = await _controller.GetById(invalidId);
            
        actionResult.Value.Should().BeNull();
        actionResult.Result.Should().BeOfType<BadRequestObjectResult>();
            
        var badResult = actionResult.Result as BadRequestObjectResult;
        badResult?.StatusCode.Should().Be(400);
        badResult?.Value.Should().BeEquivalentTo(new { Error = "Invalid ID provided." });
    }

    [Fact]
    public async Task GetById_WhenAwardProcessExists_ShouldReturn200WithDto()
    {
        const int id = 1;
        var dto = new AwardProcessResponseDto
        {
            Id = id,
            AwardsName = "Award",
            StartDate = DateTime.UtcNow,
            Status = "active",
            CreatedAt = DateTime.UtcNow
        };
        ApiResponse<AwardProcessResponseDto?, string> apiResponse = dto;
        _serviceMock.Setup(s => s.GetAwardProcessByIdAsync(id)).ReturnsAsync(apiResponse);
            
        ActionResult<AwardProcessResponseDto> actionResult = await _controller.GetById(id);
            
        actionResult.Value.Should().BeNull();
        actionResult.Result.Should().BeOfType<OkObjectResult>();
            
        var okResult = actionResult.Result as OkObjectResult;
        okResult?.StatusCode.Should().Be(200);
        okResult?.Value.Should().BeEquivalentTo(dto, options => options.ComparingByMembers<AwardProcessResponseDto>());
    }
        
    [Fact]
    public async Task GetById_WhenAwardProcessDoesNotExist_ShouldReturn404WithError()
    {
        const int id = 1;
        var errorMsg = $"AwardProcess with ID {id} not found.";
        ApiResponse<AwardProcessResponseDto?, string> apiResponse = errorMsg;
        _serviceMock.Setup(s => s.GetAwardProcessByIdAsync(id)).ReturnsAsync(apiResponse);
            
        ActionResult<AwardProcessResponseDto> actionResult = await _controller.GetById(id);
            
        actionResult.Value.Should().BeNull();
        actionResult.Result.Should().BeOfType<NotFoundObjectResult>();
            
        var notFound = actionResult.Result as NotFoundObjectResult;
        notFound?.StatusCode.Should().Be(404);
        notFound?.Value.Should().BeEquivalentTo(new { Error = errorMsg });
    }

        

    [Fact]
    public async Task Create_WhenModelStateIsInvalid_ShouldReturn400()
    {
        _controller.ModelState.AddModelError("AwardsName", "Required");
        var dto = new AwardProcessCreateDto();
            
        ActionResult<AwardProcessResponseDto> actionResult = await _controller.Create(dto);
            
        actionResult.Value.Should().BeNull();
        actionResult.Result.Should().BeOfType<BadRequestObjectResult>();
    }
        
    [Fact]
    public async Task Create_WhenValidDtoProvided_ShouldReturn201WithCreatedDto()
    {
        var createDto = new AwardProcessCreateDto
        {
            AwardsName = "New Award",
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(1),
            Status = "active"
        };
        var createdDto = new AwardProcessResponseDto
        {
            Id = 1,
            AwardsName = createDto.AwardsName,
            StartDate = createDto.StartDate,
            EndDate = createDto.EndDate,
            Status = createDto.Status,
            CreatedAt = DateTime.UtcNow
        };
        ApiResponse<AwardProcessResponseDto, string> apiResponse = createdDto;
        _serviceMock.Setup(s => s.CreateAwardProcessAsync(createDto)).ReturnsAsync(apiResponse);
            
        ActionResult<AwardProcessResponseDto> actionResult = await _controller.Create(createDto);
            
        actionResult.Value.Should().BeNull();
        actionResult.Result.Should().BeOfType<CreatedAtActionResult>();
            
        var createdResult = actionResult.Result as CreatedAtActionResult;
        createdResult?.StatusCode.Should().Be(201);
        createdResult?.ActionName.Should().Be("GetById");
        createdResult?.RouteValues?["id"].Should().Be(createdDto.Id);
        createdResult?.Value.Should().BeEquivalentTo(createdDto, options => options.ComparingByMembers<AwardProcessResponseDto>());
    }
        
    [Fact]
    public async Task Create_WhenServiceReturnsError_ShouldReturn400WithError()
    {
        var createDto = new AwardProcessCreateDto
        {
            AwardsName = "New Award",
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(1),
            Status = "active"
        };
        const string errorMsg = "Creation failed";
        ApiResponse<AwardProcessResponseDto, string> apiResponse = errorMsg;
        _serviceMock.Setup(s => s.CreateAwardProcessAsync(createDto)).ReturnsAsync(apiResponse);
            
        ActionResult<AwardProcessResponseDto> actionResult = await _controller.Create(createDto);
            
        actionResult.Value.Should().BeNull();
        actionResult.Result.Should().BeOfType<BadRequestObjectResult>();
            
        var badResult = actionResult.Result as BadRequestObjectResult;
        badResult?.StatusCode.Should().Be(400);
        badResult?.Value.Should().BeEquivalentTo(new { Error = errorMsg });
    }
        
        


    [Fact]
    public async Task Update_WhenIdIsInvalid_ShouldReturn400()
    {
        const int invalidId = 0;
        var updateDto = new AwardProcessUpdateDto
        {
            AwardsName = "Updated Award",
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(1),
            Status = "active"
        };
            
        IActionResult result = await _controller.Update(invalidId, updateDto);
            
        result.Should().BeOfType<BadRequestObjectResult>()
            .Which.Value.Should().BeEquivalentTo(new { Error = "Invalid ID provided." });
    }
        
    [Fact]
    public async Task Update_WhenModelStateIsInvalid_ShouldReturn400()
    {
        const int id = 1;
        _controller.ModelState.AddModelError("AwardsName", "Required");
        var updateDto = new AwardProcessUpdateDto(); // incomplete DTO
            
        IActionResult result = await _controller.Update(id, updateDto);
            
        result.Should().BeOfType<BadRequestObjectResult>();
    }
        
    [Fact]
    public async Task Update_WhenValidUpdateProvidedAndServiceReturnsSuccess_ShouldReturn200()
    {
        const int id = 1;
        var updateDto = new AwardProcessUpdateDto
        {
            AwardsName = "Updated Award",
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(2),
            Status = "completed"
        };
        ApiResponse<bool, string> apiResponse = true;
        _serviceMock.Setup(s => s.UpdateAwardProcessAsync(id, updateDto)).ReturnsAsync(apiResponse);
            
        IActionResult result = await _controller.Update(id, updateDto);
            
        result.Should().BeOfType<OkResult>();
    }
        
    [Fact]
    public async Task Update_WhenServiceReturnsNotFoundError_ShouldReturn404WithError()
    {
        const int id = 1;
        var updateDto = new AwardProcessUpdateDto
        {
            AwardsName = "Updated Award",
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(2),
            Status = "completed"
        };
        var errorMsg = $"AwardProcess with ID {id} not found for update.";
        ApiResponse<bool, string> apiResponse = errorMsg;
        _serviceMock.Setup(s => s.UpdateAwardProcessAsync(id, updateDto)).ReturnsAsync(apiResponse);
            
        IActionResult result = await _controller.Update(id, updateDto);
            
        result.Should().BeOfType<NotFoundObjectResult>()
            .Which.Value.Should().BeEquivalentTo(new { Error = errorMsg });
    }
        
        

    [Fact]
    public async Task Delete_WhenIdIsInvalid_ShouldReturn400()
    {
        const int invalidId = 0;
            
        IActionResult result = await _controller.Delete(invalidId);
            
        result.Should().BeOfType<BadRequestObjectResult>()
            .Which.Value.Should().BeEquivalentTo(new { Error = "Invalid ID provided." });
    }
        
    [Fact]
    public async Task Delete_WhenServiceReturnsSuccess_ShouldReturn200()
    {
        const int id = 1;
        ApiResponse<bool, string> apiResponse = true;
        _serviceMock.Setup(s => s.DeleteAwardProcessAsync(id)).ReturnsAsync(apiResponse);
            
        IActionResult result = await _controller.Delete(id);
            
        result.Should().BeOfType<OkResult>();
    }
        
    [Fact]
    public async Task Delete_WhenServiceReturnsNotFoundError_ShouldReturn404WithError()
    {
        const int id = 1;
        var errorMsg = $"AwardProcess with ID {id} not found for deletion.";
        ApiResponse<bool, string> apiResponse = errorMsg;
        _serviceMock.Setup(s => s.DeleteAwardProcessAsync(id)).ReturnsAsync(apiResponse);
            
        IActionResult result = await _controller.Delete(id);
            
        result.Should().BeOfType<NotFoundObjectResult>()
            .Which.Value.Should().BeEquivalentTo(new { Error = errorMsg });
    }
}