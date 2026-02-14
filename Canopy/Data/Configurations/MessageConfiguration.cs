using Canopy.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Canopy.Data.Configurations
{
    public class MessageConfiguration : IEntityTypeConfiguration<Message>
    {
        public void Configure(EntityTypeBuilder<Message> builder)
        {
            builder.ToTable("Message");

            builder.HasKey(m => m.Id);
            builder.Property(m => m.Id)
                   .HasColumnName("Message_Id")
                   .ValueGeneratedOnAdd();

            builder.Property(m => m.ChatId)
                   .HasColumnName("Message_Chat")
                   .IsRequired();

            builder.Property(m => m.UserId)
                   .HasColumnName("Message_User")
                   .IsRequired();

            builder.Property(m => m.Text)
                   .HasColumnName("Message_Text")
                   .IsRequired(false);

            builder.Property(m => m.Type)
                   .HasColumnName("Message_Type")
                   .IsRequired();

            builder.Property(m => m.DateCreated)
                   .HasColumnName("Message_DateCreated")
                   .IsRequired()
                   .HasDefaultValueSql("GETUTCDATE()");

            // FK to Chat – already configured in ChatConfiguration,
            // but we can still set column name / constraint name here:
            builder.HasOne(m => m.Chat)
                   .WithMany(c => c.Messages)
                   .HasForeignKey(m => m.ChatId)
                   .HasConstraintName("FK_Message_Chat")
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(m => m.MessageAttachments)
                   .WithOne(a => a.Message)
                   .HasForeignKey(a => a.MessageId)
                   .HasConstraintName("FK_MessageAttachment_Message")
                   .OnDelete(DeleteBehavior.Cascade);   // delete messages attachment when message is deleted
        }
    }
}