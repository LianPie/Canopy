using Canopy.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Canopy.Models
{
    public class Project
    {
        public int Id { get; set; }

        // FK to the creator (User)
        public int CreatorId { get; set; }
        public User Creator { get; set; } = null!;

        // Optional FK to a Group; 0 / null means “personal”
        public int? GroupId { get; set; }
        public Group? Group { get; set; }
        public bool IsActive { get; set; } = true;  
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.UtcNow;
        public DateTime? Deadline { get; set; }

        public ICollection<ProjectMember> Members { get; set; } = new List<ProjectMember>();
    }

    public class ProjectMember
    {
        public int Id { get; set; }

        public int ProjectId { get; set; }
        public Project Project { get; set; } = null!;   // navigation to Project
        public int UserId { get; set; }
        public User User { get; set; } = null!;         // navigation to User
        public bool IsActive { get; set; } = true;

        public DateTime AddedDate { get; set; } = DateTime.UtcNow;
    }
}
