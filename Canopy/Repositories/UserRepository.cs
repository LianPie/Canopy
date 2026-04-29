using Canopy.Data;
using Canopy.Helpers;
using Canopy.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Canopy.Repositories
{
    namespace TaskManager.Repositories
    {
        public class UserRepository : IUserRepository
        {
            private readonly ApplicationDbContext _ctx;
            public UserRepository(ApplicationDbContext ctx) => _ctx = ctx;

            public Task<bool> UserNameExistsAsync(string userName) =>
                _ctx.Users.AnyAsync(u => u.UserName == userName);

            public Task<bool> EmailExistsAsync(string email) =>
                _ctx.Users.AnyAsync(u => u.Email == email);

            public async Task<User> AddAsync(User user)
            {
                var entry = await _ctx.Users.AddAsync(user);
                await _ctx.SaveChangesAsync();
                return entry.Entity;
            }

            public async Task<User?> GetByUserNameOrEmailAsync(string identifier)
            {
                return await _ctx.Users
                    .FirstOrDefaultAsync(u =>
                        EF.Functions.Like(u.UserName, identifier) ||
                        EF.Functions.Like(u.Email, identifier));
            }

            public User? GetById(int Id)
            {
                return _ctx.Users
                    .FirstOrDefault(u => u.Id == Id);
            }


            public Task<bool> VerifyPasswordAsync(User user, string plainPassword) =>
                Task.FromResult(PasswordHelper.VerifyPassword(plainPassword, user.Password));





            //security actions
            public async Task<UserSecurity?> GetSecurityByUserIdAsync(int userId)
            {
                var user = await _ctx.Users
                                     .Include(u => u.UserSecurity)
                                     .FirstOrDefaultAsync(u => u.Id == userId);

                if (user != null && user.UserSecurity == null)
                {
                    user.UserSecurity = new UserSecurity { UserId= user.Id};
                    await _ctx.UserSecurity.AddAsync(user.UserSecurity);
                    await _ctx.SaveChangesAsync();

                }

                return user?.UserSecurity;
            }

            public async Task IncrementFailedAttemptsAsync(int userId)
            {
                var sec = await GetSecurityByUserIdAsync(userId);
                if (sec == null) return;

                sec.FailedLoginAttempts++;
                sec.LastFailedAttempt = DateTime.UtcNow;
                await _ctx.SaveChangesAsync();
            }

            public async Task ResetFailedAttemptsAsync(int userId)
            {
                var sec = await GetSecurityByUserIdAsync(userId);
                if (sec == null) return;

                sec.FailedLoginAttempts = 0;
                sec.LastFailedAttempt = null;
                await _ctx.SaveChangesAsync();
            }

            public async Task LockoutAsync(int userId, DateTime utcLockoutUntil)
            {
                var sec = await GetSecurityByUserIdAsync(userId);
                if (sec == null) return;

                sec.LockoutUntil = utcLockoutUntil;

                await _ctx.SaveChangesAsync();
            }

        }
    }
}
