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


            public async Task<User> UpdateAsync(User user)
            {
                _ctx.Users.Update(user);
                _ctx.SaveChanges();
                return user;
            }

            public async Task DeleteAsync(User user)
            {
                // Transfer ownership of groups created by this user to the oldest other active member
                var ownedGroups = await _ctx.Group
                    .Where(g => g.CreatorId == user.Id)
                    .ToListAsync();

                foreach (var group in ownedGroups)
                {
                    var newOwner = await _ctx.UserGroup
                        .Where(ug => ug.GroupId == group.Id && ug.UserId != user.Id && ug.IsActive)
                        .OrderBy(ug => ug.JoinedDate)
                        .Select(ug => ug.UserId)
                        .FirstOrDefaultAsync();

                    if (newOwner != 0)
                        group.CreatorId = newOwner;
                    else
                        _ctx.Group.Remove(group); // no other members — delete the group
                }

                // Transfer ownership of projects created by this user to the oldest other active member
                var ownedProjects = await _ctx.Projects
                    .Where(p => p.CreatorId == user.Id)
                    .ToListAsync();

                foreach (var project in ownedProjects)
                {
                    var newOwner = await _ctx.ProjectMember
                        .Where(pm => pm.ProjectId == project.Id && pm.UserId != user.Id && pm.IsActive)
                        .OrderBy(pm => pm.AddedDate)
                        .Select(pm => pm.UserId)
                        .FirstOrDefaultAsync();

                    if (newOwner != 0)
                        project.CreatorId = newOwner;
                    else
                        _ctx.Projects.Remove(project); // no other members — delete the project
                }

                // Delete tasks created by this user
                var createdTasks = await _ctx.PlannedTask
                    .Where(t => t.CreatorId == user.Id)
                    .ToListAsync();

                _ctx.PlannedTask.RemoveRange(createdTasks);

                // Reassign remaining tasks assigned to this user back to their creator
                var assignedTasks = await _ctx.PlannedTask
                    .Where(t => t.AssignedToUID == user.Id && t.CreatorId != user.Id)
                    .ToListAsync();

                foreach (var task in assignedTasks)
                    task.AssignedToUID = task.CreatorId;

                // Fix UserGroup rows where this user was the inviter — point InvitedBy to the member themselves
                var invitedByUser = await _ctx.UserGroup
                    .Where(ug => ug.InvitedById == user.Id && ug.UserId != user.Id)
                    .ToListAsync();

                foreach (var ug in invitedByUser)
                    ug.InvitedById = ug.UserId;

                await _ctx.SaveChangesAsync();

                // Explicitly remove this user's own group memberships so EF doesn't try to in-memory cascade
                var userMemberships = await _ctx.UserGroup
                    .Where(ug => ug.UserId == user.Id)
                    .ToListAsync();

                _ctx.UserGroup.RemoveRange(userMemberships);
                await _ctx.SaveChangesAsync();

                _ctx.Users.Remove(user);
                await _ctx.SaveChangesAsync();
            }



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
