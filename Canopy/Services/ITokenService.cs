namespace Canopy.Services
{
    public interface ITokenService
    {
        string GenerateToken(int userId, int? expiryDays = null);
        DateTime? GetTokenExpiry(string token);
        int? GetUserIdFromToken(string token);
        bool ValidateToken(string token);
    }
}
