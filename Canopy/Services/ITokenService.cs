namespace Canopy.Services
{
    public interface ITokenService
    {
        string GenerateToken(int userId, string userName, int? expiryDays = null);
        DateTime? GetTokenExpiry(string token);
        int? GetUserIdFromToken(string token);
        bool ValidateToken(string token);
    }
}
