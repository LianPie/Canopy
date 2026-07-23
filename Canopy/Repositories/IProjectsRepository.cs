using Canopy.Models;

namespace Canopy.Repositories
{
    public interface IProjectsRepository
    {
        List<Project> GetAllByUser(int userId);
        (List<Project> Items, bool HasMore) GetPage(int userId, int page, int pageSize);
        Project? GetByIdForUser(int id, int userId);
        Project Create(Project project);
        Project Update(Project project);
        void Delete(Project project);
    }
}
