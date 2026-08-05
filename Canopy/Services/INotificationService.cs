using Canopy.Models;

namespace Canopy.Services
{
    public interface INotificationService
    {
        Task SendAsync(int userId, NotificationType type, string payload);
        Task NotifyNewMessageAsync(int chatId, MessageDto message, List<int> recipientUserIds);

    }
}
