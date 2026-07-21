using Canopy.Hubs;
using Canopy.Repositories;
using Microsoft.AspNetCore.SignalR;
using System.Text.Json;
using WebPush;

namespace Canopy.Services
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _repo;
        private readonly IHubContext<NotificationHub> _hub;
        private readonly IPushSubscriptionRepository _pushRepo;
        private readonly VapidDetails _vapid;
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(
            INotificationRepository repo,
            IHubContext<NotificationHub> hub,
            IPushSubscriptionRepository pushRepo,
            IConfiguration config,
            ILogger<NotificationService> logger)
        {
            _repo = repo;
            _hub = hub;
            _pushRepo = pushRepo;
            _logger = logger;
            _vapid = new VapidDetails(
                config["VapidKeys:Subject"]!,
                config["VapidKeys:PublicKey"]!,
                config["VapidKeys:PrivateKey"]!);
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

            // Real-time (browser open)
            await _hub.Clients.User(userId.ToString()).SendAsync("ReceiveNotification", new
            {
                notification.Id,
                notification.Type,
                notification.Payload,
                notification.CreatedAt
            });

            // Push (browser closed)
            await SendPushAsync(userId, type, payload);
        }

        private async Task SendPushAsync(int userId, NotificationType type, string payload)
        {
            var subscriptions = _pushRepo.GetByUser(userId);
            if (subscriptions.Count == 0) return;

            var client = new WebPushClient();
            var pushPayload = JsonSerializer.Serialize(new { type, payload });
            var staleEndpoints = new List<string>();

            foreach (var sub in subscriptions)
            {
                try
                {
                    var pushSub = new PushSubscription(sub.Endpoint, sub.P256dh, sub.Auth);
                    await client.SendNotificationAsync(pushSub, pushPayload, _vapid);
                }
                catch (WebPushException ex) when (ex.StatusCode == System.Net.HttpStatusCode.Gone)
                {
                    // Subscription expired — clean it up
                    staleEndpoints.Add(sub.Endpoint);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to send push to endpoint {Endpoint}", sub.Endpoint);
                }
            }

            staleEndpoints.ForEach(_pushRepo.DeleteByEndpoint);
        }
    }
}
