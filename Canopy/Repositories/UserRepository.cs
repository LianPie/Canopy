using Canopy.Data;
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
                // EF Core will generate Id, DateCreated, etc.
                var entry = await _ctx.Users.AddAsync(user);
                await _ctx.SaveChangesAsync();
                return entry.Entity;
            }
        }
    }
}
