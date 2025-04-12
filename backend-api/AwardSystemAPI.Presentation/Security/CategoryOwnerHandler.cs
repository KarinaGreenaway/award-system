using AwardSystemAPI.Extensions;
using AwardSystemAPI.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authorization;

namespace AwardSystemAPI.Security;

public class CategoryOwnerHandler 
    : AuthorizationHandler<CategoryOwnerRequirement>
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IAwardCategoryRepository _categories;
    private readonly ILogger<CategoryOwnerHandler> _logger;

    public CategoryOwnerHandler(
        IHttpContextAccessor httpContextAccessor,
        IAwardCategoryRepository categories,
        ILogger<CategoryOwnerHandler> logger)
    {
        _httpContextAccessor = httpContextAccessor;
        _categories = categories;
        _logger = logger;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        CategoryOwnerRequirement requirement)
    {
        // Allow admins unconditionally
        if (context.User.IsInRole("Admin"))
        {
            context.Succeed(requirement);
            return;
        }

        // Extracting the categoryId from the route values
        var route = _httpContextAccessor.HttpContext?.GetRouteData();
        if (route == null || !route.Values.TryGetValue("categoryId", out var rawCatId))
        {
            _logger.LogWarning("categoryId route value missing.");
            return;
        }

        if (!int.TryParse(rawCatId?.ToString(), out var categoryId))
            return;

        var userId = context.User.GetUserId();
        if (userId == null)
            return;

        var category = await _categories.GetByIdAsync(categoryId);
        if (category != null && category.SponsorId == userId.Value)
            context.Succeed(requirement);
    }
}