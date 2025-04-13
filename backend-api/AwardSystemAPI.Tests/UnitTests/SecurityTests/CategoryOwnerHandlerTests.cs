using System.Security.Claims;
using AwardSystemAPI.Domain.Entities;
using AwardSystemAPI.Infrastructure.Repositories;
using AwardSystemAPI.Security;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace AwardSystemAPI.Tests.UnitTests.SecurityTests;

public class CategoryOwnerHandlerTests
{
    private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
    private readonly Mock<IAwardCategoryRepository> _categoryRepoMock;
    private readonly CategoryOwnerHandler _handler;

    public CategoryOwnerHandlerTests()
    {
        Mock<ILogger<CategoryOwnerHandler>> loggerMock = new();
        _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
        _categoryRepoMock = new Mock<IAwardCategoryRepository>();
        _handler = new CategoryOwnerHandler(
            _httpContextAccessorMock.Object,
            _categoryRepoMock.Object,
            loggerMock.Object
        );
    }

    private AuthorizationHandlerContext CreateContext(ClaimsPrincipal user)
    {
        return new AuthorizationHandlerContext(
            new[] { new CategoryOwnerRequirement() },
            user,
            resource: null
        );
    }

    private void SetupRouteValue(string categoryIdValue)
    {
        var httpContext = new DefaultHttpContext();
        var routeValues = new RouteValueDictionary { { "categoryId", categoryIdValue } };
        httpContext.Features.Set<IRouteValuesFeature>(new RouteValuesFeature { RouteValues = routeValues });
        _httpContextAccessorMock.Setup(a => a.HttpContext).Returns(httpContext);
    }

    [Fact]
    public async Task HandleRequirementAsync_AdminUser_Succeeds()
    {
        var user = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.Role, "Admin") }, "test"));
        var context = CreateContext(user);

        _httpContextAccessorMock.Setup(a => a.HttpContext).Returns(null as HttpContext);

        await _handler.HandleAsync(context);

        context.HasSucceeded.Should().BeTrue();
    }

    [Fact]
    public async Task HandleRequirementAsync_MissingRoute_DoesNotSucceed()
    {
        var user = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim("userId", "1") }, "test"));
        _httpContextAccessorMock.Setup(a => a.HttpContext).Returns(new DefaultHttpContext());

        var context = CreateContext(user);
        await _handler.HandleAsync(context);

        context.HasSucceeded.Should().BeFalse();
    }

    [Fact]
    public async Task HandleRequirementAsync_InvalidCategoryId_DoesNotSucceed()
    {
        SetupRouteValue("invalid");
        var user = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim("userId", "1") }, "test"));
        var context = CreateContext(user);

        await _handler.HandleAsync(context);

        context.HasSucceeded.Should().BeFalse();
    }

    [Fact]
    public async Task HandleRequirementAsync_MissingUserId_DoesNotSucceed()
    {
        SetupRouteValue("1");
        var user = new ClaimsPrincipal(new ClaimsIdentity()); 
        var context = CreateContext(user);

        await _handler.HandleAsync(context);

        context.HasSucceeded.Should().BeFalse();
    }

    [Fact]
    public async Task HandleRequirementAsync_CategoryNotFound_DoesNotSucceed()
    {
        SetupRouteValue("2");
        var user = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim("userId", "1") }, "test"));
        _categoryRepoMock.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(null as AwardCategory);
        var context = CreateContext(user);

        await _handler.HandleAsync(context);

        context.HasSucceeded.Should().BeFalse();
        _categoryRepoMock.Verify(r => r.GetByIdAsync(2), Times.Once);
    }

    [Fact]
    public async Task HandleRequirementAsync_SponsorMismatch_DoesNotSucceed()
    {
        SetupRouteValue("3");
        var user = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim("userId", "1") }, "test"));
        var category = new AwardCategory
        {
            Id = 3,
            SponsorId = 2,
            Name = "Category 3",
            Type = "Individual",
            ProfileStatus = "published"
        };
        _categoryRepoMock.Setup(r => r.GetByIdAsync(3)).ReturnsAsync(category);
        var context = CreateContext(user);

        await _handler.HandleAsync(context);

        context.HasSucceeded.Should().BeFalse();
    }

    [Fact]
    public async Task HandleRequirementAsync_SponsorMatches_Succeeds()
    {
        SetupRouteValue("4");
        var user = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim("userId", "4") }, "test"));
        var category = new AwardCategory
        {
            Id = 4,
            SponsorId = 4,
            Name = "Category 4",
            Type = "Team",
            ProfileStatus = "published"
        };
        _categoryRepoMock.Setup(r => r.GetByIdAsync(4)).ReturnsAsync(category);
        var context = CreateContext(user);

        await _handler.HandleAsync(context);

        context.HasSucceeded.Should().BeTrue();
    }
}