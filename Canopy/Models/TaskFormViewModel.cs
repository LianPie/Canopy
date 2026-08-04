namespace Canopy.Models
{

    public class DashboardViewData
    {
        public int CompletedCount { get; set; }
        public int OverdueCount { get; set; }
        public int UpcomingCount { get; set; }

    }

    public class TaskFormViewModel
    {
        public int? Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime? DeadLine { get; set; }
        public bool Status { get; set; }
        public bool IsEdit => Id.HasValue;


        public RecurrenceType Recurrence { get; set; } = RecurrenceType.None;
        public string? RecurrenceWeekday { get; set; }
        public int? RecurrenceMonthDay { get; set; }
        public bool IsRecurrenceEnded { get; set; }
    }

    public class TaskDataViewModel
    {
        public int Id { get; set; }
        public int? AssigneeId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime? DeadLine { get; set; }
        public bool Status { get; set; }
        public int? ProjectId { get; set; }
        public int? GroupId { get; set; }


        public RecurrenceType Recurrence { get; set; } = RecurrenceType.None;
        public string? RecurrenceWeekday { get; set; }
        public int? RecurrenceMonthDay { get; set; }
        public bool IsRecurrenceEnded { get; set; }
    }

}
