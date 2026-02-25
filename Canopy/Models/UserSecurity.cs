namespace Canopy.Models
{
    public class UserSecurity
    {
            public int Id { get; set; }

            public int UserId { get; set; }
            public User User { get; set; } = null!;

            public int FailedLoginAttempts { get; set; } = 0;
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
