using Canopy.Models;

namespace Canopy.Repositories
{
    public interface IProjectsRepository
    {
        List<Project> GetAllByUser(int userId);
        Project? GetByIdForUser(int id, int userId);
        Project Create(Project project);
        Project Update(Project project);
        void Delete(Project project);
    }
}
