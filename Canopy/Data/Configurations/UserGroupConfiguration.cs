using Canopy.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Canopy.Data.Configurations
{
    public class UserGroupConfiguration : IEntityTypeConfiguration<UserGroup>
    {
        public void Configure(EntityTypeBuilder<UserGroup> builder)
        {
            builder.ToTable("UserGroups");

            // Primary Key
            builder.HasKey(u => u.Id);
            builder.Property(u => u.Id)
                .HasColumnName("UserGroup_Id")
                .ValueGeneratedOnAdd();

            // Required Fields
            builder.Property(u => u.UserId)
                .HasColumnName("User_Id")
                .IsRequired();

            // Required Fields
            builder.Property(u => u.GroupId)
                .HasColumnName("Group_Id")
                .IsRequired();

            // Required Fields
            builder.Property(u => u.RoleInGroup)
                .HasColumnName("Role_In_Group")
                .HasMaxLength(50)
                .IsRequired();

            // Required Fields
            builder.Property(u => u.IsActive)
                .HasColumnName("Is_Active")
                .IsRequired();



        }
    }
}
