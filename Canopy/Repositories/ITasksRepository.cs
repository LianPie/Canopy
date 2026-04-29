using Canopy.Models;
using System.Threading.Tasks;

namespace Canopy.Repositories
{
    public interface ITasksRepository
    {
        List<PlannedTask> GetAllByUser(int userId);
        PlannedTask? GetByIdForUser(int id, int userId);
        PlannedTask Create(PlannedTask task);
        PlannedTask Update(PlannedTask task);
        void Delete(PlannedTask task);

    }
}
