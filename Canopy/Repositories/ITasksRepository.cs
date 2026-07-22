using Canopy.Models;
using System.Threading.Tasks;

namespace Canopy.Repositories
{
    public interface ITasksRepository
    {
        List<PlannedTask> GetAllByUser(int userId);
        List<PlannedTask> GetByDate(int userId, DateTime date);
        List<PlannedTask> GetWithoutDate(int userId);
        PlannedTask? GetByIdForUser(int id, int userId);
        PlannedTask? GetAssignedByIdForUser(int id, int userId);
        List<PlannedTask> GetByProjectId(int projectId, int userId);
        PlannedTask Create(PlannedTask task);
        PlannedTask Update(PlannedTask task);
        void Delete(PlannedTask task);
        void AddRange(List<PlannedTask> task);
        void RemoveRange(List<PlannedTask> task);

    }
}
