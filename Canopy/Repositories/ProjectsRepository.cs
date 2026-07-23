using Canopy.Data;
using Canopy.Helpers;
using Canopy.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Canopy.Repositories
{
    public class ProjectsRepository : IProjectsRepository
    {
        private readonly ApplicationDbContext _ctx;
        public ProjectsRepository(ApplicationDbContext ctx) => _ctx = ctx;

        public List<Project> GetAllByUser(int userId)
        {
            return _ctx.Projects
                .Include(p => p.Group).Where(x => x.CreatorId == userId)
                .Include(p => p.Tasks)
                .ToList();
        }

        public (List<Project> Items, bool HasMore) GetPage(int userId, int page, int pageSize)
        {
            var query = _ctx.Projects
                .Include(p => p.Group)
                .Include(p => p.Tasks)
                .Where(x => x.CreatorId == userId)
                .OrderByDescending(x => x.Id);

            var items = query.Skip((page - 1) * pageSize).Take(pageSize + 1).ToList();
            var hasMore = items.Count > pageSize;
            if (hasMore) items.RemoveAt(pageSize);
            return (items, hasMore);
        }

        public Project? GetByIdForUser(int id, int userId)
        {
            return _ctx.Projects
                .Include(p => p.Creator)
                .Include(p => p.Tasks)
                .FirstOrDefault(p => p.Id == id && p.CreatorId == userId);
        }

        public Project Create(Project project)
        {
            _ctx.Projects.Add(project);
            _ctx.SaveChanges();

            return project;
        }

        public Project Update(Project project)
        {
            _ctx.Projects.Update(project);
            _ctx.SaveChanges();

            return project;
        }

        public void Delete(Project project)
        {
            _ctx.Projects.Remove(project);
            _ctx.SaveChanges();

        }


    }
}
