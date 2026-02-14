namespace Canopy.Models
{
    public class User
    {
        public int Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string? Nickname { get; set; }
        public string? ImageUrl { get; set; }
        public string? Token { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.UtcNow;
        public DateTime? LastLogin { get; set; }
        public int Status { get; set; } = 1;


        // Navigation Properties
        public UserSecurity UserSecurity { get; set; } = null!;

        public ICollection<UserGroup> UserGroups { get; set; } = new List<UserGroup>();
        public ICollection<Group> CreatedGroups { get; set; } = new List<Group>();

        public ICollection<Project> ProjectsCreated { get; set; } = new List<Project>();
        public ICollection<ProjectMember> ProjectMemberships { get; set; } = new List<ProjectMember>();

        public ICollection<PlannedTask> TasksCreated { get; set; } = new List<PlannedTask>();
        public ICollection<PlannedTask> TaskAssignee { get; set; } = new List<PlannedTask>();
        public ICollection<MessageSeenStatus> MessageSeenStatuses { get; set; } = new List<MessageSeenStatus>();


    }
    public class UserSecurity
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public int FailedLoginAttempts { get; set; }
        public DateTime? LockoutUntil { get; set; }
        public DateTime? LastFailedAttempt { get; set; }
        public DateTime PasswordChangedDate { get; set; } = DateTime.UtcNow;
        public bool TwoFactorEnabled { get; set; } = false;
        public string? TwoFactorSecret { get; set; }
        public string RecoveryCodes { get; set; } = string.Empty;
        public bool SecurityQuestionsAnswered { get; set; } = false;
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? ModifiedDate { get; set; }


    }
}
