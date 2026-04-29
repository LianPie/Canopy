using Canopy.Models;
using System.Threading.Tasks;

namespace Canopy.Repositories
{
    public interface IUserRepository
    {
        Task<bool> UserNameExistsAsync(string userName);
        Task<bool> EmailExistsAsync(string email);
        Task<User> AddAsync(User user);
        Task<User?> GetByUserNameOrEmailAsync(string identifier);
        User? GetById(int identifier);
        Task<bool> VerifyPasswordAsync(User user, string plainPassword);


        //security
        Task<UserSecurity?> GetSecurityByUserIdAsync(int userId);
        Task IncrementFailedAttemptsAsync(int userId);
        Task ResetFailedAttemptsAsync(int userId);
        Task LockoutAsync(int userId, DateTime utcLockoutUntil);
    }
}
