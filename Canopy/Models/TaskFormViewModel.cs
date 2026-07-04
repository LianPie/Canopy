namespace Canopy.Models
{

    public class TaskFormViewModel
    {
        public int? Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime? DeadLine { get; set; }
        public bool Status { get; set; }
        public bool IsEdit => Id.HasValue;
    }

    public class TaskDataViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime? DeadLine { get; set; }
        public bool Status { get; set; }
        public int? ProjectId { get; set; }
        public int? GroupId { get; set; }
    }

}
