using Canopy.Models;

public class Notification
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User? User { get; set; }
    public NotificationType Type { get; set; }
    public string Payload { get; set; } = string.Empty;
    public bool IsRead { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public enum NotificationType
{
    GroupInvitation = 0,
    GroupInvitationAccepted = 1,
    GroupInvitationDeclined = 2,
    TaskAssigned = 3,
    ProjectAssigned = 4
}
