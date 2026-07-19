using Canopy.Models;

namespace Canopy.Repositories
{
    public interface INotificationRepository
    {
        List<Notification> GetAllByUser(int userId);
        Notification? GetByIdForUser(int id, int userId);
        Notification Create(Notification notification);
        Notification Update(Notification notification);
        void MarkAllAsRead(int userId);
        void Delete(Notification notification);
    }
}
