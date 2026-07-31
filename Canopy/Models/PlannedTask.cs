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


        //if it doesn't belong to a group Null
        public int? GroupId { get; set; }
        public Group? Group { get; set; }

        //if it doesn't belong to a project nullok
        public int? ProjectId { get; set; }
        public Project? Project { get; set; }


        //if it's personal => userId
        public int AssignedToUID { get; set; }
        public User AssignedTo { get; set; } = null!;

        public DateTime DateCreated { get; set; } = DateTime.UtcNow;
        public DateTime? DeadLine { get; set; }

        public RecurrenceType Recurrence { get; set; } = RecurrenceType.None;
        public string? RecurrenceWeekday { get; set; }
        public int? RecurrenceMonthDay { get; set; }
        public bool IsRecurrenceEnded { get; set; } = false;

        public List<TaskOccurrence> Occurrences { get; set; } = new();

    }

    public enum RecurrenceType
    {
        None = 0,
        Daily = 1,
        Weekly = 2,
        Monthly = 3
    }
}
