using System.Security.Claims;
using AwardSystemAPI.Application.DTOs.AwardCategoryDtos;
using AwardSystemAPI.Application.Services;
using AwardSystemAPI.Common;
using AwardSystemAPI.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace AwardSystemAPI.Tests.UnitTests.AwardCategoryTests;

public class AwardCategoryControllerTests
{
    private readonly Mock<IAwardCategoryService> _serviceMock;
    private readonly Mock<ILogger<AwardCategoryController>> _loggerMock;
    private readonly AwardCategoryController _controller;

    public AwardCategoryControllerTests()
    {
        _serviceMock = new Mock<IAwardCategoryService>();
        _loggerMock = new Mock<ILogger<AwardCategoryController>>();
        _controller = new AwardCategoryController(_serviceMock.Object, _loggerMock.Object);
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
    public async Task GetAll_WhenServiceReturnsSuccess_ShouldReturnOkWithDtos()
    {
        var dtos = new List<AwardCategoryResponseDto>
        {
            new AwardCategoryResponseDto { Id = 1, Name = "Category1", Type="individual", SponsorId = 1, ProfileStatus = "draft", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new AwardCategoryResponseDto { Id = 2, Name = "Category2", Type="team", SponsorId = 1, ProfileStatus = "published", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
        };
        ApiResponse<IEnumerable<AwardCategoryResponseDto>, string> apiResponse = dtos;
        _serviceMock.Setup(s => s.GetAllAwardCategoriesAsync()).ReturnsAsync(apiResponse);

        ActionResult<IEnumerable<AwardCategoryResponseDto>> result = await _controller.GetAll();

        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        okResult?.Value.Should().BeEquivalentTo(dtos);
    }

    [Fact]
    public async Task GetAll_WhenServiceReturnsError_ShouldReturnBadRequest()
    {
        const string errorMsg = "Error retrieving categories";
        ApiResponse<IEnumerable<AwardCategoryResponseDto>, string> apiResponse = errorMsg;
        _serviceMock.Setup(s => s.GetAllAwardCategoriesAsync()).ReturnsAsync(apiResponse);

        ActionResult<IEnumerable<AwardCategoryResponseDto>> result = await _controller.GetAll();

        result.Result.Should().BeOfType<BadRequestObjectResult>();
        var badResult = result.Result as BadRequestObjectResult;
        badResult?.Value.Should().BeEquivalentTo(new { Error = errorMsg });
    }

    [Fact]
    public async Task GetById_WithInvalidId_ShouldReturnBadRequest()
    {
        // Arrange
        int invalidId = 0;

        // Act
        ActionResult<AwardCategoryResponseDto> result = await _controller.GetById(invalidId);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
        var badResult = result.Result as BadRequestObjectResult;
        badResult?.Value.Should().BeEquivalentTo(new { Error = "Invalid ID provided." });
    }

    [Fact]
    public async Task GetById_WhenCategoryExists_ShouldReturnOkWithDto()
    {
        // Arrange
        int id = 1;
        var dto = new AwardCategoryResponseDto { Id = id, Name = "Category1", Type="individual", SponsorId = 1, ProfileStatus = "draft", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow };
        ApiResponse<AwardCategoryResponseDto?, string> apiResponse = dto;
        _serviceMock.Setup(s => s.GetAwardCategoryByIdAsync(id)).ReturnsAsync(apiResponse);

        // Act
        ActionResult<AwardCategoryResponseDto> result = await _controller.GetById(id);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        okResult?.Value.Should().BeEquivalentTo(dto);
    }

    [Fact]
    public async Task GetById_WhenCategoryDoesNotExist_ShouldReturnNotFound()
    {
        // Arrange
        int id = 1;
        string errorMsg = $"AwardCategory with ID {id} not found.";
        ApiResponse<AwardCategoryResponseDto?, string> apiResponse = errorMsg;
        _serviceMock.Setup(s => s.GetAwardCategoryByIdAsync(id)).ReturnsAsync(apiResponse);

        // Act
        ActionResult<AwardCategoryResponseDto> result = await _controller.GetById(id);

        // Assert
        result.Result.Should().BeOfType<NotFoundObjectResult>();
        var notFound = result.Result as NotFoundObjectResult;
        notFound?.Value.Should().BeEquivalentTo(new { Error = errorMsg });
    }


    [Fact]
    public async Task GetMyCategories_WhenUserIdClaimMissing_ShouldReturnUnauthorized()
    {
        // Arrange: make User return null for GetUserId.
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext() // no claims assigned
        };

        // Act
        ActionResult<IEnumerable<AwardCategoryResponseDto>> result = await _controller.GetMyCategories();

        // Assert
        result.Result.Should().BeOfType<UnauthorizedObjectResult>();
    }

    [Fact]
    public async Task GetMyCategories_WhenServiceReturnsSuccess_ShouldReturnOkWithDtos()
    {
        // Arrange: simulate a user with userId = 1.
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = CreateUserWithId("1") }
        };

        var dtos = new List<AwardCategoryResponseDto>
        {
            new AwardCategoryResponseDto { Id = 1, Name = "Category1", Type="individual", SponsorId = 1, ProfileStatus = "draft", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
        };
        ApiResponse<IEnumerable<AwardCategoryResponseDto>, string> apiResponse = dtos;
        _serviceMock.Setup(s => s.GetAwardCategoriesBySponsorIdAsync(1)).ReturnsAsync(apiResponse);

        // Act
        ActionResult<IEnumerable<AwardCategoryResponseDto>> result = await _controller.GetMyCategories();

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        okResult?.Value.Should().BeEquivalentTo(dtos);
    }

    [Fact]
    public async Task GetMyCategories_WhenServiceReturnsError_ShouldReturnBadRequest()
    {
        // Arrange: simulate a user with userId = 1.
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = CreateUserWithId("1") }
        };

        string errorMsg = "Error retrieving categories";
        ApiResponse<IEnumerable<AwardCategoryResponseDto>, string> apiResponse = errorMsg;
        _serviceMock.Setup(s => s.GetAwardCategoriesBySponsorIdAsync(1)).ReturnsAsync(apiResponse);

        // Act
        ActionResult<IEnumerable<AwardCategoryResponseDto>> result = await _controller.GetMyCategories();

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
        var badResult = result.Result as BadRequestObjectResult;
        badResult?.Value.Should().BeEquivalentTo(new { Error = errorMsg });
    }

    [Fact]
    public async Task Create_WhenModelStateIsInvalid_ShouldReturnBadRequest()
    {
        // Arrange
        _controller.ModelState.AddModelError("Name", "Name is required");
        var dto = new AwardCategoryCreateDto();

        // Act
        ActionResult<AwardCategoryResponseDto> result = await _controller.Create(dto);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task Create_WhenUserIdClaimMissing_ShouldReturnUnauthorized()
    {
        // Arrange: No user claim assigned.
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };
        var dto = new AwardCategoryCreateDto { Name = "New Cat" };

        // Act
        ActionResult<AwardCategoryResponseDto> result = await _controller.Create(dto);

        // Assert
        result.Result.Should().BeOfType<UnauthorizedObjectResult>();
    }

    [Fact]
    public async Task Create_WhenServiceReturnsSuccess_ShouldReturnCreatedAtAction()
    {
        // Arrange: simulate a user with userId = 1.
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = CreateUserWithId("1") }
        };

        var inputDto = new AwardCategoryCreateDto { Name = "New Cat" };
        // The service sets SponsorId internally, so we expect it to be 1.
        var createdDto = new AwardCategoryResponseDto { Id = 1, Name = "New Cat", SponsorId = 1, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow };
        ApiResponse<AwardCategoryResponseDto, string> apiResponse = createdDto;
        _serviceMock.Setup(s => s.CreateAwardCategoryAsync(It.IsAny<AwardCategoryCreateDto>())).ReturnsAsync(apiResponse);

        // Act
        ActionResult<AwardCategoryResponseDto> result = await _controller.Create(inputDto);

        // Assert
        result.Result.Should().BeOfType<CreatedAtActionResult>();
        var createdAt = result.Result as CreatedAtActionResult;
        createdAt?.RouteValues?["id"].Should().Be(createdDto.Id);
        createdAt?.Value.Should().BeEquivalentTo(createdDto);
    }

    [Fact]
    public async Task Create_WhenServiceReturnsError_ShouldReturnBadRequest()
    {
        // Arrange: simulate a user with userId = 1.
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = CreateUserWithId("1") }
        };

        var dto = new AwardCategoryCreateDto { Name = "New Cat" };
        string errorMsg = "Creation failed";
        ApiResponse<AwardCategoryResponseDto, string> apiResponse = errorMsg;
        _serviceMock.Setup(s => s.CreateAwardCategoryAsync(It.IsAny<AwardCategoryCreateDto>())).ReturnsAsync(apiResponse);

        // Act
        ActionResult<AwardCategoryResponseDto> result = await _controller.Create(dto);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
        var badResult = result.Result as BadRequestObjectResult;
        badResult?.Value.Should().BeEquivalentTo(new { Error = errorMsg });
    }


    [Fact]
    public async Task Update_WhenModelStateIsInvalid_ShouldReturnBadRequest()
    {
        _controller.ModelState.AddModelError("Name", "Required");
        var dto = new AwardCategoryUpdateDto();
        const int id = 1;

        IActionResult result = await _controller.Update(id, dto);

        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task Update_WhenServiceReturnsSuccess_ShouldReturnOk()
    {
        var dto = new AwardCategoryUpdateDto { Name = "Updated Cat" };
        const int id = 1;
        ApiResponse<bool, string> apiResponse = true;
        _serviceMock.Setup(s => s.UpdateAwardCategoryAsync(id, dto)).ReturnsAsync(apiResponse);

        IActionResult result = await _controller.Update(id, dto);

        result.Should().BeOfType<OkResult>();
    }

    [Fact]
    public async Task Update_WhenServiceReturnsNotFoundError_ShouldReturnNotFound()
    {
        var dto = new AwardCategoryUpdateDto { Name = "Updated Cat" };
        const int id = 1;
        var errorMsg = $"AwardCategory with ID {id} not found for update.";
        ApiResponse<bool, string> apiResponse = errorMsg;
        _serviceMock.Setup(s => s.UpdateAwardCategoryAsync(id, dto)).ReturnsAsync(apiResponse);

        IActionResult result = await _controller.Update(id, dto);

        result.Should().BeOfType<NotFoundObjectResult>();
        var notFound = result as NotFoundObjectResult;
        notFound?.Value.Should().BeEquivalentTo(new { Error = errorMsg });
    }


    [Fact]
    public async Task Delete_WhenServiceReturnsSuccess_ShouldReturnOk()
    {
        const int id = 1;
        ApiResponse<bool, string> apiResponse = true;
        _serviceMock.Setup(s => s.DeleteAwardCategoryAsync(id)).ReturnsAsync(apiResponse);

        IActionResult result = await _controller.Delete(id);

        result.Should().BeOfType<OkResult>();
    }

    [Fact]
    public async Task Delete_WhenServiceReturnsNotFoundError_ShouldReturnNotFound()
    {
        const int id = 1;
        var errorMsg = $"AwardCategory with ID {id} not found for deletion.";
        ApiResponse<bool, string> apiResponse = errorMsg;
        _serviceMock.Setup(s => s.DeleteAwardCategoryAsync(id)).ReturnsAsync(apiResponse);

        IActionResult result = await _controller.Delete(id);

        result.Should().BeOfType<NotFoundObjectResult>();
        var notFound = result as NotFoundObjectResult;
        notFound?.Value.Should().BeEquivalentTo(new { Error = errorMsg });
    }
}