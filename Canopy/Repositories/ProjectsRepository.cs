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
            //TO DO: modify this to also acount for projects from groups
            return _ctx.Projects.Where(x => x.CreatorId == userId)
                .ToList();
        }

        public Project? GetByIdForUser(int id, int userId)
        {
            return _ctx.Projects
                .Include(p => p.Creator)
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
