using Canopy.Models;

public class Group
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public int CreatorId { get; set; }
    public User? Creator { get; set; }
    public DateTime DateCreated { get; set; }
    public ICollection<UserGroup> UserGroups { get; set; } = new List<UserGroup>();
    public ICollection<Project> Projects { get; set; } = new List<Project>();   
    public ICollection<PlannedTask> Tasks { get; set; } = new List<PlannedTask>();   

}

public class UserGroup
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User? User { get; set; }
    public int GroupId { get; set; }
    public Group? Group { get; set; }
    public int InvitedById { get; set; }
    public User? InvitedBy { get; set; }
    public DateTime InvitedAt { get; set; }
    public DateTime? JoinedDate { get; set; }
    public string? RoleInGroup { get; set; }
    public bool IsActive { get; set; }
    public InvitationStatus Status { get; set; } = InvitationStatus.Pending;
}

public enum InvitationStatus
{
    Pending = 0,
    Accepted = 1,
    Declined = 2
}

