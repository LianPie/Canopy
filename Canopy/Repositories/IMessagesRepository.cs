using Canopy.Models;

namespace Canopy.Repositories
{
    public interface IMessagesRepository
    {
        Task<List<Message>> GetByChatIdAsync(int chatId, int skip = 0, int take = 50);
        Task<Message> CreateAsync(Message message);
        Task MarkAsSeenAsync(int messageId, int userId);
    }

}
