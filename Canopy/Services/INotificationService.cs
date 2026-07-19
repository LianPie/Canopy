namespace Canopy.Services
{
    public interface INotificationService
    {
        Task SendAsync(int userId, NotificationType type, string payload);
    }
}
