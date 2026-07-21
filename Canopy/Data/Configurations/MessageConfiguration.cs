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
                   .HasDefaultValueSql("CURRENT_TIMESTAMP");

            builder.HasOne(m => m.User)
                   .WithMany()
                   .HasForeignKey(m => m.UserId)
                   .HasConstraintName("FK_Message_User")
                   .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
