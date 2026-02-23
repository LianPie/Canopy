using Canopy.Models;
using System.Threading.Tasks;

namespace Canopy.Repositories
{
    public interface IUserRepository
    {
        Task<bool> UserNameExistsAsync(string userName);
        Task<bool> EmailExistsAsync(string email);
        Task<User> AddAsync(User user);
    }
}
