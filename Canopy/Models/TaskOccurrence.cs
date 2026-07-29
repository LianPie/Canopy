namespace Canopy.Models
{
    public class TaskOccurrence
    {
        public int Id { get; set; }
        public int TaskId { get; set; }
        public PlannedTask? Task { get; set; } = null;
        public DateTime OccurrenceDate { get; set; } // which day this checkmark is for
        public bool IsCompleted { get; set; }
        public DateTime? CompletedAt { get; set; }
    }
}
