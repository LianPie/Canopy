using Canopy.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Canopy.Data.Configurations
{
    public class MessageSeenStatusConfiguration
        : IEntityTypeConfiguration<MessageSeenStatus>
    {
        public void Configure(EntityTypeBuilder<MessageSeenStatus> builder)
        {
            builder.ToTable("MessageSeenStatus");

            builder.HasKey(m => new { m.MessageId, m.UserId });

            builder.Property(m => m.MessageId)
                   .HasColumnName("Message_Id")
                   .IsRequired();

            builder.Property(m => m.UserId)
                   .HasColumnName("User_Id")
                   .IsRequired();

            builder.Property(m => m.SeenDate)
                   .HasColumnName("Seen_Date")
                   .IsRequired()
                   .HasDefaultValueSql("CURRENT_TIMESTAMP");  

            // Relationships (optional but recommended)
            builder.HasOne(m => m.Message)
                   .WithMany(msg => msg.SeenStatuses)   // add ICollection<MessageSeenStatus> on Message
                   .HasForeignKey(m => m.MessageId)
                   .HasConstraintName("FK_MessageSeenStatus_Message")
                   .OnDelete(DeleteBehavior.Cascade);   // delete rows if a message is removed

            builder.HasOne(m => m.User)
                   .WithMany(u => u.MessageSeenStatuses) // add ICollection<MessageSeenStatus> on User
                   .HasForeignKey(m => m.UserId)
                   .HasConstraintName("FK_MessageSeenStatus_User")
                   .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
