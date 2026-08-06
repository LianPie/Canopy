using Canopy.Models;

namespace Canopy.Services
{
    public interface IChatService
    {
        Task<List<int>> GetUserChatIdsAsync(int userId);
        Task<bool> IsUserMemberOfChatAsync(int chatId, int userId);
        Task<MessageDto> SendMessageAsync(int chatId, int userId, string text);
        Task MarkMessageAsSeenAsync(int messageId, int userId, int chatId);
        Task<List<MessageDto>> GetMessagesAsync(int chatId, int skip = 0, int take = 50);
        Task<Chat> GetOrCreateChatForGroupAsync(int groupId);


    }
}
