using Canopy.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Canopy.Data.Configurations
{
    public class PlannedTaskConfiguration : IEntityTypeConfiguration<PlannedTask>
    {
        public void Configure(EntityTypeBuilder<PlannedTask> builder)
        {
            builder.ToTable("PlannedTask");

            // Primary key
            builder.HasKey(t => t.Id);
            builder.Property(t => t.Id)
                   .HasColumnName("Task_Id")
                   .ValueGeneratedOnAdd();

            // foreign key mapping
            builder.Property(t => t.CreatorId)
                   .HasColumnName("Task_Creator")
                   .IsRequired();

            builder.Property(t => t.GroupId)
                   .HasColumnName("Task_Group")
                   .IsRequired(false);

            builder.Property(t => t.ProjectId)
                   .HasColumnName("Task_Project")
                   .IsRequired(false);

            builder.Property(t => t.AssignedToUID)
                   .HasColumnName("Task_AssignedTo")
                   .IsRequired();

            builder.Property(t => t.Title)
                .HasColumnName("Task_Title")
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(t => t.Description)
                .HasColumnName("Task_Description")
                .HasMaxLength(255)
                .IsRequired(false);

            builder.Property(t => t.Status)
                .HasColumnName("Task_Status")
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(t => t.DateCreated)
                .HasColumnName("Task_DateCreated")
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(t => t.DeadLine)
                .HasColumnName("Task_DeadLine")
                .IsRequired(false);


            //navihation properties
            builder.HasOne(t => t.Creator)
                .WithMany(u => u.TasksCreated)
                .HasForeignKey(t => t.CreatorId)
                .HasConstraintName("FK_PlannedTask_User")
                .OnDelete(DeleteBehavior.Restrict); 

            builder.HasOne(t => t.AssignedTo)
                .WithMany(u => u.TaskAssignee)
                .HasForeignKey(t => t.AssignedToUID)
                .HasConstraintName("FK_PlannedTask_AssignedTo")
                .OnDelete(DeleteBehavior.Restrict); 

            builder.HasOne(t => t.Group)
                .WithMany(g => g.Tasks)
                .HasForeignKey(t => t.GroupId)
                .HasConstraintName("FK_PlannedTask_Group")
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(t => t.Project)
                .WithMany(g => g.Tasks)
                .HasForeignKey(t => t.ProjectId)
                .HasConstraintName("FK_PlannedTask_Project")
                .OnDelete(DeleteBehavior.Restrict);


            builder.HasIndex(t => t.CreatorId);
        }
    }
}
