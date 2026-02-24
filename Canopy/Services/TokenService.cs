using Canopy.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Canopy.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<TokenService> _logger;
        private readonly SymmetricSecurityKey _key;

        // Can inject ANY dependencies!
        public TokenService(IConfiguration config, ILogger<TokenService> logger)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            var secret = _config["JwtSettings:Secret"]
                         ?? throw new InvalidOperationException(
                               "JWT secret is missing in configuration.");

            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        }

        public string GenerateToken(int userId, int? expiryDays = null)
        {
            _logger.LogInformation($"Generating token for user {userId}"); 

            var claims = new[]
            {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim("generated_at", DateTime.UtcNow.ToString())
        };

            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha256);

            var expiry = expiryDays.HasValue
                ? DateTime.UtcNow.AddDays(expiryDays.Value)
                : DateTime.UtcNow.AddHours(1);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: expiry,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public DateTime? GetTokenExpiry(string token)
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);
                _logger.LogDebug($"Token expires at {jwtToken.ValidTo}");
                return jwtToken.ValidTo;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to read token expiry");
                return null;
            }
        }

        public int? GetUserIdFromToken(string token)
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);
                var userIdClaim = jwtToken.Claims
                    .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

                return userIdClaim != null ? int.Parse(userIdClaim.Value) : null;
            }
            catch
            {
                return null;
            }
        }

        public bool ValidateToken(string token)
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var parameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = _key,
                    ValidateIssuer = true,
                    ValidIssuer = _config["Jwt:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = _config["Jwt:Audience"],
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };

                handler.ValidateToken(token, parameters, out _);
                _logger.LogInformation("Token validated successfully");
                return true;
            }
            catch (SecurityTokenExpiredException)
            {
                _logger.LogWarning("Token expired");
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Token validation failed");
                return false;
            }
        }
    }
}
