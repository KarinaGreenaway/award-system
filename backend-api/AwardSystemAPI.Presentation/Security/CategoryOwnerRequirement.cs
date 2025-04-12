using Microsoft.AspNetCore.Authorization;

namespace AwardSystemAPI.Security;

public class CategoryOwnerRequirement : IAuthorizationRequirement
{
    // no payload needed
}