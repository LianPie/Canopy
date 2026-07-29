using Canopy.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Canopy.Data.Configurations
{
    public class TaskOccurrenceConfiguration : IEntityTypeConfiguration<TaskOccurrence>
    {
        public void Configure(EntityTypeBuilder<TaskOccurrence> builder)
        {
            builder.ToTable("TaskOccurrence");

            builder.HasKey(t => t.Id);
            builder.Property(t => t.Id)
                   .HasColumnName("TaskOccurrence_Id")
                   .ValueGeneratedOnAdd();

            builder.Property(t => t.TaskId)
                   .HasColumnName("TaskOccurrence_Task")
                   .IsRequired();

            builder.Property(t => t.OccurrenceDate)
                .HasColumnName("TaskOccurrence_OccurrenceDate")
                .IsRequired();
            // no default — always set explicitly to the day being tracked

            builder.Property(t => t.IsCompleted)
                .HasColumnName("TaskOccurrence_IsCompleted")
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(t => t.CompletedAt)
                .HasColumnName("TaskOccurrence_CompletedAt")
                .IsRequired(false);
            // nullable, set in code only when IsCompleted becomes true

            builder.HasOne(t => t.Task)
                .WithMany(pt => pt.Occurrences)
                .HasForeignKey(t => t.TaskId)
                .HasConstraintName("FK_PlannedTaskOccurrence_Task")
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(t => new { t.TaskId, t.OccurrenceDate })
                   .IsUnique();
        }
    }
}