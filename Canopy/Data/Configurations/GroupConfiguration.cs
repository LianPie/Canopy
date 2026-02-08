using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Canopy.Data.Configurations
{
    public class GroupConfiguration
    {
        public void Configure(EntityTypeBuilder<Group> builder)
        {
            builder.ToTable("Group");

            // Primary Key
            builder.HasKey(u => u.Id);
            builder.Property(u => u.Id)
                .HasColumnName("Group_Id")
                .ValueGeneratedOnAdd();

            // Required Fields
            builder.Property(u => u.Title)
                .HasColumnName("Group_Title")
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(u => u.Creator)
                .HasColumnName("Group_Creator")
                .IsRequired();

            builder.Property(u => u.DateCreated)
                .HasColumnName("Group_DateCreated")
                .IsRequired();


            builder.HasOne(g => g.Creator)           //  Group -> a Creator 
                .WithMany(u => u.CreatedGroups)     // a User => multiple Groups
                .HasForeignKey(g => g.CreatorId)    // Foreign Key
                .OnDelete(DeleteBehavior.Restrict); // when User is deleted -> don't delete Group

        }
    }
}
