using Canopy.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Canopy.Data.Configurations
{
    public class UserSecurityConfiguration : IEntityTypeConfiguration<UserSecurity>
    {
        public void Configure(EntityTypeBuilder<UserSecurity> builder)
        {
            // نام جدول
            builder.ToTable("UserSecurity");

            // Primary Key
            builder.HasKey(us => us.Id);
            builder.Property(us => us.Id)
                .HasColumnName("UserSecurity_Id")
                .ValueGeneratedOnAdd();


            builder.Property(us => us.UserId)
                .HasColumnName("User_Id")
                .IsRequired();

            // Required Fields
            builder.Property(us => us.FailedLoginAttempts)
                .HasColumnName("Failed_Login_Attempts")
                .IsRequired();

            builder.Property(us => us.LockoutUntil)
                .HasColumnName("Lockout_Until")
                .IsRequired(false);

            builder.Property(us => us.LastFailedAttempt)
                .HasColumnName("Last_Failed_Attempt")
                .IsRequired(false);

            builder.Property(us => us.PasswordChangedDate)
                .HasColumnName("Password_Changed_Date")
                .HasDefaultValueSql("GETUTCDATE()")
                .IsRequired();
                
            builder.Property(us => us.TwoFactorEnabled)
                .HasColumnName("Two_Factor_Enabled")
                .IsRequired();

            // Nullable Fields
            builder.Property(us => us.TwoFactorSecret)
                .HasColumnName("Two_Factor_Secret")
                .HasMaxLength(255)
                .IsRequired(false);

            builder.Property(us => us.RecoveryCodes)
                .HasColumnName("Recovery_Codes")
                .IsRequired(false);

            builder.Property(us => us.SecurityQuestionsAnswered)
                .HasColumnName("Security_Questions_Answered")
                .IsRequired();

            // DateTime Fields
            builder.Property(us => us.CreatedDate)
                .HasColumnName("Created_Date")
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()"); // یا برای SQL Server

            builder.Property(us => us.ModifiedDate)
                .HasColumnName("Modified_Date")
                .HasDefaultValueSql("GETUTCDATE()")
                .IsRequired();

            builder.HasOne(us => us.User)             
                .WithOne(x => x.UserSecurity)
                .HasForeignKey<UserSecurity>(us => us.UserId)
                .OnDelete(DeleteBehavior.Cascade);  



            // Indexes
            builder.HasIndex(us => us.Id).IsUnique();
            builder.HasIndex(us => us.UserId).IsUnique();
        }
    }
}
