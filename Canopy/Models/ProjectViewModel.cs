namespace Canopy.Models
{
    public class ProjectViewModel
    {
        public int? Id { get; set; }
        public string CreatorName { get; set; } = string.Empty;
        public bool Status { get; set; } = true;
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.UtcNow;
        public DateTime? Deadline { get; set; }
        public ICollection<PlannedTask> Tasks { get; set; } = new List<PlannedTask>();
        public bool IsEdit => Id.HasValue;
    }
    public class ProjectDataViewModel
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool Status { get; set; } = true;
        public int? Group { get; set; }
        public DateTime? Deadline { get; set; }
        public ICollection<TaskDataViewModel> Tasks { get; set; } = new List<TaskDataViewModel>();
    }
}
