using Canopy.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Canopy.Data.Configurations
{
    public class MessageAttachmentConfiguration
        : IEntityTypeConfiguration<MessageAttachment>
    {
        public void Configure(EntityTypeBuilder<MessageAttachment> builder)
        {
            builder.ToTable("MessageAttachments");

            builder.HasKey(a => a.Id);
            builder.Property(a => a.Id)
                   .HasColumnName("Attachment_Id")
                   .ValueGeneratedOnAdd();

            builder.Property(a => a.MessageId)
                   .HasColumnName("Message_Id")
                   .IsRequired();

            builder.Property(a => a.FilePath)
                   .HasColumnName("File_Path")
                   .HasMaxLength(500)
                   .IsRequired();

            builder.Property(a => a.Name)
                   .HasColumnName("File_Name")
                   .HasMaxLength(255)
                   .IsRequired();

            builder.Property(a => a.Size)
                   .HasColumnName("File_Size_Bytes")
                   .IsRequired(false);

            builder.Property(a => a.MimeType)
                   .HasColumnName("Mime_Type")
                   .HasMaxLength(100)
                   .IsRequired(false);

            builder.Property(a => a.DateUploaded)
                   .HasColumnName("Uploaded_Date")
                   .IsRequired()
                   .HasDefaultValueSql("GETUTCDATE()");  

            builder.HasOne(a => a.Message)
                   .WithMany(m => m.MessageAttachments)
                   .HasForeignKey(a => a.MessageId)
                   .HasConstraintName("FK_MessageAttachment_Message")
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}