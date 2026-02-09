using Canopy.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Canopy.Data.Configurations
{
    public class ProjectMemberConfiguration : IEntityTypeConfiguration<ProjectMember>
    {
        public void Configure(EntityTypeBuilder<ProjectMember> builder)
        {
            builder.ToTable("ProjectMember");

            // Primary key
            builder.HasKey(pm => pm.Id);
            builder.Property(pm => pm.Id)
                   .HasColumnName("ProjectMember_Id")
                   .ValueGeneratedOnAdd();

            builder.Property(pm => pm.IsActive)
                   .HasColumnName("Is_Active")
                   .IsRequired()
                   .HasDefaultValue(true);

            builder.Property(pm => pm.AddedDate)
                   .HasColumnName("Added_Date")
                   .IsRequired()
                   .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(pm => pm.ProjectId)
                   .HasColumnName("Project_Id")
                   .IsRequired();

            builder.Property(pm => pm.UserId)
                   .HasColumnName("User_Id")
                   .IsRequired();

            // Many‑to‑one: ProjectMember → Project
            builder.HasOne(pm => pm.Project)
                   .WithMany(p => p.Members)          // navigation collection on Project
                   .HasForeignKey(pm => pm.ProjectId)
                   .HasConstraintName("FK_ProjectMember_Project")
                   .OnDelete(DeleteBehavior.Cascade); // delete memberships when a project is removed

            // Many‑to‑one: ProjectMember → User
            builder.HasOne(pm => pm.User)
                   .WithMany(u => u.ProjectMemberships) // navigation collection on User
                   .HasForeignKey(pm => pm.UserId)
                   .HasConstraintName("FK_ProjectMember_User")
                   .OnDelete(DeleteBehavior.Cascade); // delete memberships when  a user is removed


            builder.HasIndex(pm => new { pm.ProjectId, pm.UserId })
                   .IsUnique()
                   .HasDatabaseName("IX_ProjectMember_Unique_Project_User");

            builder.HasIndex(pm => pm.IsActive);
        }
    }
}