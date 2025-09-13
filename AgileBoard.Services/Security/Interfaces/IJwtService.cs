using AgileBoard.Domain.Entities;

namespace AgileBoard.Services.Security.Interfaces
{
    public interface IJwtService
    {
        string GenerateToken(User user);
        bool ValidateToken(string token);
        int? GetUserIdFromToken(string token);
        string? GetUsernameFromToken(string token);
    }
}
