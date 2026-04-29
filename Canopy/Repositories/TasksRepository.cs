using Canopy.Data;
using Canopy.Helpers;
using Canopy.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Canopy.Repositories
{
    public class TasksRepository : ITasksRepository
    {
        private readonly ApplicationDbContext _ctx;
        public TasksRepository(ApplicationDbContext ctx) => _ctx = ctx;

        public async Task<List<Project>> GetUserProjectsAsync(int UserId)
        {
            var projects = await _ctx.ProjectMember.Include(m => m.Project).Where(m => m.UserId == UserId)
                .Select(x => x.Project)
                .ToListAsync();

            return projects;
        }

        public async Task<List<Group>> GetUserGroupsAsync(int UserId)
        {
            var Groups = await _ctx.UserGroup.Include(m => m.Group).Where(m => m.UserId == UserId)
                .Select(x => x.Group)
                .ToListAsync();

            return Groups;
        }
        public async Task<List<PlannedTask>> GetUserTasksAsync(int UserId)
        {
            var tasks = await _ctx.PlannedTask.Where(x => x.AssignedToUID == UserId).ToListAsync();
            return tasks;
        }
        public async Task<List<PlannedTask>> GetProjectTasksAsync(int UserId)
        {
            var tasks = await _ctx.PlannedTask.Include(t => t.Project)
                .ThenInclude(p => p.Members)
                .Where(t => t.Project != null && t.Project.Members.Any(m => m.UserId == UserId))
                .ToListAsync();

            return tasks;
        }
        public async Task<List<PlannedTask>> GetGroupTasksAsync(int UserId)
        {
            var tasks = await _ctx.PlannedTask.Include(t => t.Group)
                .ThenInclude(g => g.UserGroups)
                .Where(t => t.Group != null && t.Group.UserGroups.Any(m => m.UserId == UserId))
                .ToListAsync();

            return tasks;
        }
    }
}
