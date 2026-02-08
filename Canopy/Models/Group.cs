using Canopy.Models;

public class Group
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public int CreatorId { get; set; }
    public User? Creator { get; set; }
    public DateTime DateCreated { get; set; }
    public ICollection<UserGroup> UserGroups { get; set; } = new List<UserGroup>();

}

public class UserGroup
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User? User { get; set; }
    public int GroupId { get; set; }
    public Group? Group { get; set; }
    public DateTime JoinedDate { get; set; }
    public string? RoleInGroup { get; set; }
    public bool IsActive { get; set; }
}

