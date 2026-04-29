using Canopy.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Canopy.Models
{
    public class PlannedTask
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool Status { get; set; } = true;

        public int CreatorId { get; set; }
        public User Creator { get; set; } = null!;


        //if it doesn't belong to a group => 0
        public int? GroupId { get; set; }
        public Group? Group { get; set; }

        //if it doesn't belong to a project => 0
        public int? ProjectId { get; set; }
        public Project? Project { get; set; }


        //if it's personal => userId
        public int AssignedToUID { get; set; }
        public User AssignedTo { get; set; } = null!;

        public DateTime DateCreated { get; set; } = DateTime.UtcNow;
        public DateTime? DeadLine { get; set; }

    }
}
