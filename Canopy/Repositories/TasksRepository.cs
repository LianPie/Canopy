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
                .Where(x => x.AssignedToUID == userId && x.DeadLine.HasValue && x.DeadLine.Value.Date == date.Date && x.Recurrence == RecurrenceType.None)
                .ToList();
            return tasks;
        }

        public List<PlannedTask> GetWithoutDate(int userId)
        {
            return _ctx.PlannedTask
                .Include(p => p.Project)
                .Include(p => p.Group)
                .Where(x => x.AssignedToUID == userId && !x.DeadLine.HasValue && x.Recurrence == RecurrenceType.None)
                .ToList();
        }

        public List<PlannedTask> Getreaccuring(int userId)
        {
            return _ctx.PlannedTask
                .Include(p => p.Project)
                .Include(p => p.Group)
                .Include(p => p.Occurrences)
                .Where(x => x.AssignedToUID == userId && x.Recurrence != RecurrenceType.None)
                .ToList();
        }

        public List<PlannedTask> GetreaccuringforToday(int userId)
        {
            var today = DateTime.Today;
            var currentWeekday = today.DayOfWeek.ToString(); // DayOfWeek enum (e.g., DayOfWeek.Tuesday)
            var currentDayOfMonth = today.Day;    // Integer (e.g., 4)

            return _ctx.PlannedTask
                .Include(p => p.Project)
                .Include(p => p.Group)
                .Include(p => p.Occurrences)
                .Where(x => x.AssignedToUID == userId
                         && x.Recurrence != RecurrenceType.None
                         && !x.IsRecurrenceEnded
                         && (
                             x.Recurrence == RecurrenceType.Daily
                             || (x.Recurrence == RecurrenceType.Weekly && x.RecurrenceWeekday == currentWeekday)
                             || (x.Recurrence == RecurrenceType.Monthly && x.RecurrenceMonthDay == currentDayOfMonth)
                         ))
                .ToList();
        }

        public DashboardViewData GetDashboardStats(int userId)
        {
            var today = DateTime.Today;

            var stats = _ctx.PlannedTask
                .Where(x => x.AssignedToUID == userId || x.CreatorId == userId)
                .GroupBy(x => 1)
                .Select(g => new DashboardViewData
                {
                    CompletedCount = g.Count(x => x.Status == true),
                    OverdueCount = g.Count(x => x.Status == false && x.DeadLine.HasValue && x.DeadLine.Value.Date < today),
                    UpcomingCount = g.Count(x => x.Status == false && x.DeadLine.HasValue && x.DeadLine.Value.Date > today)
                })
                .FirstOrDefault();

            return (stats);
        }
        public (List<PlannedTask> Items, bool HasMore) GetPage(bool? isOverdue, int userId, int page, int pageSize)
        {
            var today = DateTime.Today;
            var baseQuery = _ctx.PlannedTask
                .Include(x => x.Project)
                .Include(x => x.Group)
                .Where(x => (x.AssignedToUID == userId) && x.Status == false && x.Recurrence == RecurrenceType.None);

            IQueryable<PlannedTask> query;

            if (isOverdue.HasValue)
            {
                query = isOverdue.Value
                    ? baseQuery.Where(x => x.DeadLine.HasValue && x.DeadLine.Value.Date < today).OrderBy(x => x.DeadLine)
                    : baseQuery.Where(x => x.DeadLine.HasValue && x.DeadLine.Value.Date >= today).OrderBy(x => x.DeadLine); 
            }
            else
            {
                query = baseQuery.Where(x => !x.DeadLine.HasValue).OrderByDescending(x => x.DateCreated); 
            }

            var items = query.Skip((page - 1) * pageSize).Take(pageSize + 1).ToList();
            var hasMore = items.Count > pageSize;

            if (hasMore)
                items.RemoveAt(pageSize);

            return (items, hasMore);
        }
        public List<int> GetAllUserIdsWithTasks()
        {
            return _ctx.PlannedTask
                .Where(t => !t.Status && t.AssignedToUID > 0)
                .Select(t => t.AssignedToUID)
                .Distinct()
                .ToList();
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


        public void OccuranceCheck(int id, DateTime OccurrenceDate)
        {
            var reOccurance = _ctx.TaskOccurrence.FirstOrDefault(x => x.TaskId == id && x.OccurrenceDate == OccurrenceDate);
            if (reOccurance == null)
            {
                TaskOccurrence task = new TaskOccurrence()
                {
                    TaskId = id,
                    CompletedAt = DateTime.Now,
                    IsCompleted = true,
                    OccurrenceDate = OccurrenceDate,
                };
                _ctx.TaskOccurrence.Add(task);
            }
            else
            {
                reOccurance.IsCompleted = !reOccurance.IsCompleted;
                _ctx.TaskOccurrence.Update(reOccurance);
            }

            _ctx.SaveChanges();

        }
    }
}
