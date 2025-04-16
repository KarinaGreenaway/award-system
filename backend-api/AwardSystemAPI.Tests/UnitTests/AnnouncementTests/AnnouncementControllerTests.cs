using System.Security.Claims;
using AwardSystemAPI.Application.DTOs;
using AwardSystemAPI.Application.Services;
using AwardSystemAPI.Common;
using AwardSystemAPI.Controllers;
using AwardSystemAPI.Domain.Enums;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace AwardSystemAPI.Tests.UnitTests.AnnouncementTests;

public class AnnouncementControllerTests
{
    private readonly Mock<IAnnouncementService> _serviceMock;
    private readonly Mock<ILogger<AnnouncementController>> _loggerMock;
    private readonly AnnouncementController _controller;

    public AnnouncementControllerTests()
    {
        _serviceMock = new Mock<IAnnouncementService>();
        _loggerMock  = new Mock<ILogger<AnnouncementController>>();
        _controller  = new AnnouncementController(_serviceMock.Object, _loggerMock.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            }
        };
    }

    private static ClaimsPrincipal CreateUser(string userId) =>
        new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim("userId", userId) }, "Test"));


    [Fact]
    public async Task Create_InvalidModel_ReturnsBadRequest()
    {
        _controller.ModelState.AddModelError("Title", "Required");
        var dto = new AnnouncementCreateDto();

        var result = await _controller.Create(dto);

        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task Create_NoUserClaim_ReturnsUnauthorized()
    {
        var dto = new AnnouncementCreateDto();
        // no user set on HttpContext.User

        var result = await _controller.Create(dto);

        var unauthorized = result.Result as UnauthorizedObjectResult;
        unauthorized?.StatusCode.Should().Be(StatusCodes.Status401Unauthorized);
        unauthorized?.Value.Should().Be("User ID is missing from the token.");
    }

    [Fact]
    public async Task Create_ServiceSuccess_ReturnsCreated()
    {
        var dto = new AnnouncementCreateDto();
        var created = new AnnouncementResponseDto { Id = 5 };
        ApiResponse<AnnouncementResponseDto, string> resp = created;

        _controller.ControllerContext.HttpContext.User = CreateUser("42");
        _serviceMock
            .Setup(s => s.CreateAnnouncementAsync(dto, 42))
            .ReturnsAsync(resp);

        var action = await _controller.Create(dto);
        var result = action.Result as CreatedAtActionResult;

        result?.StatusCode.Should().Be(StatusCodes.Status201Created);
        result?.ActionName.Should().Be(nameof(_controller.GetById));
        result?.RouteValues?["id"].Should().Be(5);
        result?.Value.Should().BeEquivalentTo(created);
    }

    [Fact]
    public async Task Create_ServiceError_ReturnsBadRequest()
    {
        var dto = new AnnouncementCreateDto();
        ApiResponse<AnnouncementResponseDto, string> resp = "oops";

        _controller.ControllerContext.HttpContext.User = CreateUser("42");
        _serviceMock
            .Setup(s => s.CreateAnnouncementAsync(dto, 42))
            .ReturnsAsync(resp);

        var action = await _controller.Create(dto);
        var objectResult = action.Result as BadRequestObjectResult;

        objectResult?.Value.Should().BeEquivalentTo(new { Error = "oops" });
    }

    [Fact]
    public async Task GetAll_ServiceSuccess_ReturnsOk()
    {
        var list = new List<AnnouncementResponseDto> { new() { Id = 1 } };
        ApiResponse<IEnumerable<AnnouncementResponseDto>, string> resp = list;

        _controller.ControllerContext.HttpContext.User = CreateUser("42");
        _serviceMock.Setup(s => s.GetAllAnnouncementsAsync()).ReturnsAsync(resp);

        var result = await _controller.GetAll();
        var objectResult = result.Result as OkObjectResult;

        objectResult?.Value.Should().BeEquivalentTo(list);
    }

    [Fact]
    public async Task GetAll_ServiceError_ReturnsBadRequest()
    {
        ApiResponse<IEnumerable<AnnouncementResponseDto>, string> resp = "fail";

        _controller.ControllerContext.HttpContext.User = CreateUser("42");
        _serviceMock.Setup(s => s.GetAllAnnouncementsAsync()).ReturnsAsync(resp);

        var result = await _controller.GetAll();
        var bad = result.Result as BadRequestObjectResult;

        bad?.Value.Should().BeEquivalentTo(new { Error = "fail" });
    }

    [Fact]
    public async Task GetMine_NoUser_ReturnsUnauthorized()
    {
        var result = await _controller.GetMine();
        result.Result.Should().BeOfType<UnauthorizedObjectResult>();
    }

    [Fact]
    public async Task GetMine_ServiceSuccess_ReturnsOk()
    {
        var list = new List<AnnouncementResponseDto> { new() { Id = 2 } };
        ApiResponse<IEnumerable<AnnouncementResponseDto>, string> resp = list;

        _controller.ControllerContext.HttpContext.User = CreateUser("7");
        _serviceMock.Setup(s => s.GetMyCategoryAnnouncementsAsync(7)).ReturnsAsync(resp);

        var result = await _controller.GetMine();
        (result.Result as OkObjectResult)?.Value.Should().BeEquivalentTo(list);
    }

    [Fact]
    public async Task GetMine_ServiceError_ReturnsBadRequest()
    {
        ApiResponse<IEnumerable<AnnouncementResponseDto>, string> resp = "err";

        _controller.ControllerContext.HttpContext.User = CreateUser("7");
        _serviceMock.Setup(s => s.GetMyCategoryAnnouncementsAsync(7)).ReturnsAsync(resp);

        var bad = await _controller.GetMine();
        (bad.Result as BadRequestObjectResult)?.Value.Should().BeEquivalentTo(new { Error = "err" });
    }

    [Fact]
    public async Task GetBySponsor_ServiceSuccess_ReturnsOk()
    {
        var list = new List<AnnouncementResponseDto> { new() { Id = 3 } };
        ApiResponse<IEnumerable<AnnouncementResponseDto>, string> resp = list;
        
        _serviceMock.Setup(s => s.GetAnnouncementsByCreatorIdAsync(99)).ReturnsAsync(resp);

        var result = await _controller.GetBySponsor(99);
        (result.Result as OkObjectResult)?.Value.Should().BeEquivalentTo(list);
    }

    [Fact]
    public async Task GetBySponsor_ServiceError_ReturnsBadRequest()
    {
        ApiResponse<IEnumerable<AnnouncementResponseDto>, string> resp = "bad";

        _serviceMock.Setup(s => s.GetAnnouncementsByCreatorIdAsync(5)).ReturnsAsync(resp);
        var result = await _controller.GetBySponsor(5);

        (result.Result as BadRequestObjectResult)?.Value.Should().BeEquivalentTo(new { Error = "bad" });
    }

    [Fact]
    public async Task GetById_InvalidId_ReturnsBadRequest()
    {
        var result = await _controller.GetById(0);
        (result.Result as BadRequestObjectResult)?.Value
            .Should().BeEquivalentTo(new { Error = "Invalid ID provided." });
    }

    [Fact]
    public async Task GetById_NotFound_ReturnsNotFound()
    {
        ApiResponse<AnnouncementResponseDto?, string> resp = "not found";

        _serviceMock.Setup(s => s.GetAnnouncementByIdAsync(12)).ReturnsAsync(resp);
        var result = await _controller.GetById(12);

        (result.Result as NotFoundObjectResult)?.Value.Should().BeEquivalentTo(new { Error = "not found" });
    }

    [Fact]
    public async Task GetById_ServiceErrorOther_ReturnsBadRequest()
    {
        ApiResponse<AnnouncementResponseDto?, string> resp = "other error";

        _serviceMock.Setup(s => s.GetAnnouncementByIdAsync(8)).ReturnsAsync(resp);
        var result = await _controller.GetById(8);

        (result.Result as BadRequestObjectResult)?.Value.Should().BeEquivalentTo(new { Error = "other error" });
    }

    [Fact]
    public async Task GetById_Success_ReturnsOk()
    {
        var dto = new AnnouncementResponseDto { Id = 42 };
        ApiResponse<AnnouncementResponseDto?, string> resp = dto;

        _serviceMock.Setup(s => s.GetAnnouncementByIdAsync(42)).ReturnsAsync(resp);
        var result = await _controller.GetById(42);

        (result.Result as OkObjectResult)?.Value.Should().BeEquivalentTo(dto);
    }

    [Fact]
    public async Task GetMobile_ServiceSuccess_ReturnsOk()
    {
        var list = new List<AnnouncementResponseDto> { new() { Id = 9 } };
        ApiResponse<IEnumerable<AnnouncementResponseDto>, string> resp = list;

        _serviceMock.Setup(s => s.GetPublishedForAudienceAsync(TargetAudience.MobileUsers))
            .ReturnsAsync(resp);

        var ok = await _controller.GetMobile();
        (ok.Result as OkObjectResult)?.Value.Should().BeEquivalentTo(list);
    }

    [Fact]
    public async Task GetMobile_ServiceError_ReturnsBadRequest()
    {
        ApiResponse<IEnumerable<AnnouncementResponseDto>, string> resp = "fail mobile";

        _serviceMock.Setup(s => s.GetPublishedForAudienceAsync(TargetAudience.MobileUsers))
            .ReturnsAsync(resp);

        var bad = await _controller.GetMobile();
        (bad.Result as BadRequestObjectResult)?.Value.Should().BeEquivalentTo(new { Error = "fail mobile" });
    }

    [Fact]
    public async Task GetSponsor_ServiceSuccess_ReturnsOk()
    {
        var list = new List<AnnouncementResponseDto> { new() { Id = 13 } };
        ApiResponse<IEnumerable<AnnouncementResponseDto>, string> resp = list;

        _serviceMock.Setup(s => s.GetPublishedForAudienceAsync(TargetAudience.Sponsors))
            .ReturnsAsync(resp);

        var ok = await _controller.GetSponsor();
        (ok.Result as OkObjectResult)?.Value.Should().BeEquivalentTo(list);
    }

    [Fact]
    public async Task GetSponsor_ServiceError_ReturnsBadRequest()
    {
        ApiResponse<IEnumerable<AnnouncementResponseDto>, string> resp = "fail sponsor";

        _serviceMock.Setup(s => s.GetPublishedForAudienceAsync(TargetAudience.Sponsors))
            .ReturnsAsync(resp);

        var bad = await _controller.GetSponsor();
        (bad.Result as BadRequestObjectResult)?.Value.Should().BeEquivalentTo(new { Error = "fail sponsor" });
    }

    [Fact]
    public async Task Update_InvalidModel_ReturnsBadRequest()
    {
        _controller.ModelState.AddModelError("Any", "err");
        var result = await _controller.Update(5, new AnnouncementUpdateDto());
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task Update_NoUser_ReturnsUnauthorized()
    {
        var result = await _controller.Update(5, new AnnouncementUpdateDto());
        result.Should().BeOfType<UnauthorizedObjectResult>();
    }

    [Fact]
    public async Task Update_NotFound_ReturnsNotFound()
    {
        ApiResponse<bool, string> resp = "not found";

        _controller.ControllerContext.HttpContext.User = CreateUser("3");
        _serviceMock.Setup(s => s.UpdateAnnouncementAsync(7, It.IsAny<AnnouncementUpdateDto>(), 3))
            .ReturnsAsync(resp);

        var result = await _controller.Update(7, new AnnouncementUpdateDto());
        (result as NotFoundObjectResult)?.Value.Should().BeEquivalentTo(new { Error = "not found" });
    }

    [Fact]
    public async Task Update_ServiceErrorOther_ReturnsBadRequest()
    {
        ApiResponse<bool, string> resp = "other error";

        _controller.ControllerContext.HttpContext.User = CreateUser("8");
        _serviceMock.Setup(s => s.UpdateAnnouncementAsync(8, It.IsAny<AnnouncementUpdateDto>(), 8))
            .ReturnsAsync(resp);

        var result = await _controller.Update(8, new AnnouncementUpdateDto());
        (result as BadRequestObjectResult)?.Value.Should().BeEquivalentTo(new { Error = "other error" });
    }

    [Fact]
    public async Task Update_Success_ReturnsOk()
    {
        ApiResponse<bool, string> resp = true;

        _controller.ControllerContext.HttpContext.User = CreateUser("21");
        _serviceMock.Setup(s => s.UpdateAnnouncementAsync(21, It.IsAny<AnnouncementUpdateDto>(), 21))
            .ReturnsAsync(resp);

        var result = await _controller.Update(21, new AnnouncementUpdateDto());
        result.Should().BeOfType<OkResult>();
    }

    [Fact]
    public async Task Delete_NoUser_ReturnsUnauthorized()
    {
        var result = await _controller.Delete(1);
        result.Should().BeOfType<UnauthorizedObjectResult>();
    }

    [Fact]
    public async Task Delete_NotFound_ReturnsNotFound()
    {
        ApiResponse<bool, string> resp = "not found";

        _controller.ControllerContext.HttpContext.User = CreateUser("4");
        _serviceMock.Setup(s => s.DeleteAnnouncementAsync(9, 4))
            .ReturnsAsync(resp);

        var result = await _controller.Delete(9);
        (result as NotFoundObjectResult)?.Value.Should().BeEquivalentTo(new { Error = "not found" });
    }

    [Fact]
    public async Task Delete_ServiceErrorOther_ReturnsBadRequest()
    {
        ApiResponse<bool, string> resp = "other";

        _controller.ControllerContext.HttpContext.User = CreateUser("4");
        _serviceMock.Setup(s => s.DeleteAnnouncementAsync(6, 4))
            .ReturnsAsync(resp);

        var result = await _controller.Delete(6);
        (result as BadRequestObjectResult)?.Value.Should().BeEquivalentTo(new { Error = "other" });
    }

    [Fact]
    public async Task Delete_Success_ReturnsOk()
    {
        ApiResponse<bool, string> resp = true;

        _controller.ControllerContext.HttpContext.User = CreateUser("13");
        _serviceMock.Setup(s => s.DeleteAnnouncementAsync(13, 13))
            .ReturnsAsync(resp);

        var result = await _controller.Delete(13);
        result.Should().BeOfType<OkResult>();
    }
}