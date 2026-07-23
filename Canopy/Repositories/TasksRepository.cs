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

        public List<PlannedTask> GetByDate(int userId, DateTime date)
        {
            var tasks = _ctx.PlannedTask
                .Include(p => p.Project)
                .Include(p => p.Group)
                .Where(x => x.AssignedToUID == userId && x.DeadLine.HasValue)
                .ToList();
            return tasks.Where(x => x.DeadLine.Value.Date == date.Date).ToList();
        }

        public List<PlannedTask> GetWithoutDate(int userId)
        {
            return _ctx.PlannedTask
                .Include(p => p.Project)
                .Include(p => p.Group)
                .Where(x => x.AssignedToUID == userId && !x.DeadLine.HasValue)
                .ToList();
        }


        public (List<PlannedTask> Items, bool HasMore) GetPage(bool isOverdue, int userId, int page, int pageSize)
        {
            var today = DateTime.Today;
            var base_ = _ctx.PlannedTask
                .Include(x => x.Project)
                .Include(x => x.Group)
                .Where(x => (x.AssignedToUID == userId || x.CreatorId == userId) && x.Status == false && x.DeadLine.HasValue);

            var query = isOverdue
                ? base_.Where(x => x.DeadLine!.Value.Date < today).OrderBy(x => x.DeadLine)
                : base_.Where(x => x.DeadLine!.Value.Date > today).OrderBy(x => x.DeadLine);

            var items = query.Skip((page - 1) * pageSize).Take(pageSize + 1).ToList();
            var hasMore = items.Count > pageSize;
            if (hasMore) items.RemoveAt(pageSize);
            return (items, hasMore);
        }



        public PlannedTask? GetByIdForUser(int id, int userId)
        {
            return _ctx.PlannedTask
                .FirstOrDefault(t => t.Id == id && t.CreatorId == userId);
        }

        public PlannedTask? GetAssignedByIdForUser(int id, int userId)
        {
            return _ctx.PlannedTask
                .FirstOrDefault(t => t.Id == id && (t.AssignedToUID == userId || t.CreatorId == userId));
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
