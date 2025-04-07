using System.Security.Claims;

namespace AwardSystemAPI.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static int? GetUserId(this ClaimsPrincipal user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));
                
            var claimValue = user.FindFirst("userId")?.Value; 
            // TODO: Change the name looked for in the claim to the correct one once integrated with the existing Microsoft authentication setup.
            
            return int.TryParse(claimValue, out var id) ? id : null;
        }
    }
}