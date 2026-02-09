using Canopy.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Canopy.Data.Configurations
{
    public class ProjectConfiguration : IEntityTypeConfiguration<Project>
    {
        public void Configure(EntityTypeBuilder<Project> builder)
        {
            // Table name
            builder.ToTable("Project");

            // Primary key
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Id)
                   .HasColumnName("Project_Id")
                   .ValueGeneratedOnAdd();

            builder.Property(p => p.CreatorId)
                  .HasColumnName("Creator_Id")
                  .IsRequired();

            builder.Property(p => p.GroupId)
                   .HasColumnName("Group_Id")
                   .IsRequired(false);


            builder.Property(p => p.IsActive)
                   .HasColumnName("Project_IsActive")
                   .IsRequired()
                   .HasDefaultValue(true);               // defaults to active

            builder.Property(p => p.Title)
                   .HasColumnName("Project_Title")
                   .HasMaxLength(50)
                   .IsRequired();

            builder.Property(p => p.Description)
                   .HasColumnName("Project_Description")
                   .HasMaxLength(255)                     // matches your original spec
                   .IsRequired(false);

            builder.Property(p => p.DateCreated)
                   .HasColumnName("Project_DateCreated")
                   .IsRequired()
                   .HasDefaultValueSql("GETUTCDATE()"); // SQL Server UTC timestamp

            builder.Property(p => p.Deadline)
                   .HasColumnName("Project_Deadline")
                   .IsRequired(false);



            builder.HasOne(p => p.Creator)
                   .WithMany(u => u.ProjectsCreated)
                   .HasForeignKey(p => p.CreatorId)
                   .HasConstraintName("FK_Project_User")
                   .OnDelete(DeleteBehavior.Restrict);   // don’t cascade delete projects when a user is removed

            // 2️⃣ Group – optional many‑to‑one
            builder.HasOne(p => p.Group)
                   .WithMany(g => g.Projects)
                   .HasForeignKey(p => p.GroupId)
                   .HasConstraintName("FK_Project_Group")
                   .OnDelete(DeleteBehavior.SetNull);    // clear GroupId if the group is deleted


            builder.HasIndex(p => p.Title);
            builder.HasIndex(p => p.CreatorId);
            builder.HasIndex(p => p.GroupId);
        }
    }
}