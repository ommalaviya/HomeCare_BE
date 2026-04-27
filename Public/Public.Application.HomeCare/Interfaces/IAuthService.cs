using Shared.HomeCare.Entities;

namespace Public.Application.HomeCare.Interfaces
{
    public interface IAuthService
    {
        string GenerateToken(User user);
        string GenerateJwtToken(User user);
        string GenerateRefreshToken();
    }
}