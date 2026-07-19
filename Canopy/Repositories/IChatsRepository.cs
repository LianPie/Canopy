using Canopy.Models;

namespace Canopy.Repositories
{
    public interface IChatsRepository
    {
        List<Chat> GetByIdForUser(int id, int userId);
        Chat Create(Chat chat);
        Chat Update(Chat chat);
        void Delete(Chat chat);
    }
}
