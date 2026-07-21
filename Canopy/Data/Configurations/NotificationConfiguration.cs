using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Canopy.Data.Configurations
{
    public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {
            builder.ToTable("Notifications");

            builder.HasKey(n => n.Id);
            builder.Property(n => n.Id)
                .HasColumnName("Notification_Id")
                .ValueGeneratedOnAdd();

            builder.Property(n => n.UserId)
                .HasColumnName("User_Id")
                .IsRequired();

            builder.Property(n => n.Type)
                .HasColumnName("Notification_Type")
                .IsRequired();

            builder.Property(n => n.Payload)
                .HasColumnName("Notification_Payload")
                .HasMaxLength(1000)
                .IsRequired();

            builder.Property(n => n.IsRead)
                .HasColumnName("Is_Read")
                .IsRequired();

            builder.Property(n => n.CreatedAt)
                .HasColumnName("Created_At")
                .IsRequired();

            builder.HasOne(n => n.User)
                .WithMany()
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

