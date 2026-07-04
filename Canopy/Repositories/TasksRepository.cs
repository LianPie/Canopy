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

        public List<PlannedTask> GetAllByUser(int userId)
        {
            return _ctx.PlannedTask
                .Include(p => p.Project)
                .Include(p => p.Group)
                .Where(x => x.AssignedToUID == userId)
                .ToList();
        }

        public PlannedTask? GetByIdForUser(int id, int userId)
        {
            return _ctx.PlannedTask
                .FirstOrDefault(t => t.Id == id && t.CreatorId == userId);
        }

        public PlannedTask Create(PlannedTask task)
        {
            _ctx.PlannedTask.Add(task);
            _ctx.SaveChanges();

            return task;
        }

        public PlannedTask Update(PlannedTask task)
        {
            _ctx.PlannedTask.Update(task);
            _ctx.SaveChanges();

            return task;
        }

        public void Delete(PlannedTask task)
        {
            _ctx.PlannedTask.Remove(task);
            _ctx.SaveChanges();

        }


        //Project Tasks
        public void AddRange(List<PlannedTask> task)
        {
            _ctx.PlannedTask.AddRange(task);
            _ctx.SaveChanges();
        }
        public void RemoveRange(List<PlannedTask> task)
        {
            _ctx.PlannedTask.RemoveRange(task);
            _ctx.SaveChanges();
        }
        public List<PlannedTask> GetByProjectId(int projectId, int userId)
        {
            return _ctx.PlannedTask
                .Where(t => t.ProjectId == projectId && t.Project.CreatorId == userId)
                .ToList();
        }
    }
}
