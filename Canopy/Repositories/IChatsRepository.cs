using Canopy.Models;

namespace Canopy.Repositories
{
    public interface IChatsRepository
    {
        Task<List<int>> GetChatIdsForUserAsync(int userId);
        Task<Chat?> GetByIdAsync(int chatId);
        Task<Chat?> GetByGroupIdAsync(int groupId);
        Task<List<Chat>>  GetByIdForUser(int id, int userId);
        Task<Chat> Create(Chat chat);
        Task<Chat> Update(Chat chat);
        Task Delete(Chat chat);
    }
}
