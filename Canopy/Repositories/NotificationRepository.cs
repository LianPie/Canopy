using Canopy.Data;
using Canopy.Helpers;
using Canopy.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Canopy.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly ApplicationDbContext _ctx;
        public NotificationRepository(ApplicationDbContext ctx) => _ctx = ctx;

        public List<Notification> GetPageByUser(int userId, int page, int pageSize)
        {
            return _ctx.Notifications
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();
        }

        public int GetUnreadCount(int userId)
        {
            return _ctx.Notifications.Count(x => x.UserId == userId && !x.IsRead);
        }

        public Notification? GetByIdForUser(int id, int userId)
        {
            return _ctx.Notifications
                .FirstOrDefault(p => p.Id == id && p.UserId == userId);
        }

        public Notification Create(Notification notification)
        {
            _ctx.Notifications.Add(notification);
            _ctx.SaveChanges();

            return notification;
        }

        public Notification Update(Notification notification)
        {
            _ctx.Notifications.Update(notification);
            _ctx.SaveChanges();

            return notification;
        }

        public void MarkAllAsRead(int userId)
        {
            _ctx.Notifications
            .Where(n => n.UserId == userId && !n.IsRead)
            .ExecuteUpdate(s => s.SetProperty(n => n.IsRead, true));

        }

        public void Delete(Notification notification)
        {
            _ctx.Notifications.Remove(notification);
            _ctx.SaveChanges();

        }


    }
}
