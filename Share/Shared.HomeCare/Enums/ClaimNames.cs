using System.Security.Claims;

namespace Shared.HomeCare.Enums
{
    public static class ClaimNames
    {
        public const string Id = ClaimTypes.NameIdentifier;

        /// <summary>Custom claim — not remapped, stays as "name"</summary>
        public const string Name = "name";

        /// <summary>Custom claim — not remapped, stays as "isSuperAdmin"</summary>
        public const string IsSuperAdmin = "isSuperAdmin";

        /// <summary>Custom claim — not remapped, stays as "isActive"</summary>
        public const string IsDeleted = "IsDeleted";
    }
}