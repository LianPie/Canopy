using Canopy.Models;

namespace Canopy.Repositories
{
    public interface INotificationRepository
    {
        List<Notification> GetPageByUser(int userId, int page, int pageSize);
        int GetUnreadCount(int userId);
        Notification? GetByIdForUser(int id, int userId);
        Notification Create(Notification notification);
        Notification Update(Notification notification);
        void MarkAllAsRead(int userId);
        void Delete(Notification notification);
    }
}
