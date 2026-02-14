using Canopy.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Canopy.Data.Configurations
{
    public class ChatConfiguration : IEntityTypeConfiguration<Chat>
    {
        public void Configure(EntityTypeBuilder<Chat> builder)
        {
            builder.ToTable("Chat");

            // Primary Key
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Id)
                .HasColumnName("Chat_Id")
                .ValueGeneratedOnAdd();

            builder.Property(c => c.GroupId)
                .HasColumnName("Chat_Group")
                .IsRequired();

            builder.Property(c => c.DateStarted)
                .HasColumnName("Chat_DateStarted")
                .HasDefaultValueSql("GETUTCDATE()")
                .IsRequired();

            builder.Property(c => c.IsActive)
                .HasColumnName("Chat_IsActive")
                .IsRequired();

            //nav property
            builder.HasMany(c => c.Messages)
                   .WithOne(m => m.Chat)
                   .HasForeignKey(m => m.ChatId)
                   .HasConstraintName("FK_Message_Chat")
                   .OnDelete(DeleteBehavior.Cascade);   // delete messages when a chat is deleted

        }
    }
}
