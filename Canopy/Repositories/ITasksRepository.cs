using Canopy.Models;
using System.Threading.Tasks;

namespace Canopy.Repositories
{
    public interface ITasksRepository
    {
        public Task<List<Project>> GetUserProjectsAsync(int UserId);
        public Task<List<PlannedTask>> GetUserTasksAsync(int UserId);
    }
}
