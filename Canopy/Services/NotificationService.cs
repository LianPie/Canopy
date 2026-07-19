using Canopy.Hubs;
using Canopy.Repositories;
using Microsoft.AspNetCore.SignalR;

namespace Canopy.Services
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _repo;
        private readonly IHubContext<NotificationHub> _hub;

        public NotificationService(INotificationRepository repo, IHubContext<NotificationHub> hub)
        {
            _repo = repo;
            _hub = hub;
        }

        public async Task SendAsync(int userId, NotificationType type, string payload)
        {
            var notification = _repo.Create(new Notification
            {
                UserId = userId,
                Type = type,
                Payload = payload,
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            });

            await _hub.Clients.User(userId.ToString()).SendAsync("ReceiveNotification", new
            {
                notification.Id,
                notification.Type,
                notification.Payload,
                notification.CreatedAt
            });
        }
    }
}
