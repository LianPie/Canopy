using Canopy.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Canopy.Data.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("User");

            // Primary Key
            builder.HasKey(u => u.Id);
            builder.Property(u => u.Id)
                .HasColumnName("User_Id")
                .ValueGeneratedOnAdd();

            // Required Fields
            builder.Property(u => u.UserName)
                .HasColumnName("User_UserName")
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(u => u.Email)
                .HasColumnName("User_Email")
                .HasMaxLength(255)
                .IsRequired();

            builder.Property(u => u.Password)
                .HasColumnName("User_Password")
                .HasMaxLength(255)
                .IsRequired();

            // Nullable Fields
            builder.Property(u => u.Nickname)
                .HasColumnName("User_Nickname")
                .HasMaxLength(100)
                .IsRequired(false);

            builder.Property(u => u.ImageUrl)
                .HasColumnName("User_Img")
                .HasMaxLength(500)
                .IsRequired(false);

            builder.Property(u => u.Token)
                .HasColumnName("User_Token")
                .HasMaxLength(500)
                .IsRequired(false);

            // DateTime Fields
            builder.Property(u => u.DateCreated)
                .HasColumnName("User_DateCreated")
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(u => u.LastLogin)
                .HasColumnName("User_LastLogin")
                .IsRequired(false);

            // Status
            builder.Property(u => u.Status)
                .HasColumnName("User_Status")
                .IsRequired()
                .HasDefaultValue(1);


            // 1.  User nav property
            builder.HasMany(u => u.CreatedGroups)          // a User -> multiple Groups 
                .WithOne(g => g.Creator)                   // a Group -> one Creator
                .HasForeignKey(g => g.CreatorId)           // Foreign Key in Group
                .OnDelete(DeleteBehavior.Restrict);        // whern User is deleted -> don't delete Groups

            // 2. User nav property with UserGroup)
            builder.HasMany(u => u.UserGroups)             // a User - >multiple UserGroups
                .WithOne(ug => ug.User)                    // a UserGroup -> one User 
                .HasForeignKey(ug => ug.UserId)           // Foreign Key in UserGroup
                .OnDelete(DeleteBehavior.Cascade);         // when User is deleted -> delete UserGroups



            // Indexes
            builder.HasIndex(u => u.UserName).IsUnique();
            builder.HasIndex(u => u.Email).IsUnique();

            
        }
    }
}
