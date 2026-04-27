using System.Security.Claims;
using Shared.HomeCare.Enums;
using System.IdentityModel.Tokens;
namespace Shared.HomeCare.Extensions
{
    /// <summary>
    /// Extension methods for reading typed values from JWT claims.
    /// /// </summary>
    public static class ClaimsExtension
    {
        /// <summary>Returns admin.Id (JWT "sub" claim). Returns 0 when absent.</summary>
        public static int GetAdminId(this ClaimsPrincipal principal)
        {
            var value = principal.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            return int.TryParse(value, out var id) ? id : 0;
        }

        /// <summary>Returns user.Id (JWT "sub" claim). Returns 0 when absent.</summary>
        public static int GetUserId(this ClaimsPrincipal principal)
        {
           var value = principal.FindFirst(JwtRegisteredClaimNames.Sub)?.Value 
                     ?? principal.FindFirst(ClaimNames.Id)?.Value;
            return int.TryParse(value, out var id) ? id : 0;
        }

        /// <summary>Returns name claim. Returns empty string when absent.</summary>
        public static string GetName(this ClaimsPrincipal principal)
            => principal.FindFirst(ClaimNames.Name)?.Value ?? string.Empty;

        /// <summary>Returns admin.IsSuperAdmin. Returns false when absent.</summary>
        public static bool GetIsSuperAdmin(this ClaimsPrincipal principal)
        {
            var value = principal.FindFirst(ClaimNames.IsSuperAdmin)?.Value;
            return bool.TryParse(value, out var result) && result;
        }

        /// <summary>Returns IsDeleted flag. Returns false when absent.</summary>
        public static bool GetIsDeleted(this ClaimsPrincipal principal)
        {
            var value = principal.FindFirst(ClaimNames.IsDeleted)?.Value;
            return bool.TryParse(value, out var result) && result;
        }
    }
}