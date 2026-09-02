using System.ComponentModel.DataAnnotations;

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
        public string? EmailVerificationCode { get; set; }
        public DateTime? VerificationCodeExpiry { get; set; }
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
    public class ProfileViewModel
    {
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? ProfilePictureUrl { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? LastLoginAt { get; set; }
    }
    public class ChangePasswordRequest
    {
        public string OldPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
        public string ConfirmNewPassword { get; set; } = string.Empty;
    }

    public class VerifyEmailViewModel
    {
        [Required]
        public string Email { get; set; }

        [Required]
        [StringLength(6, MinimumLength = 6, ErrorMessage = "Code must be 6 digits.")]
        public string Code { get; set; }
    }

    public class DeleteAccountRequestModel
    {
        public string Code { get; set; } = string.Empty;
    }
}
