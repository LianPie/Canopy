using Canopy.Data.Configurations;
using Canopy.Models;
using Microsoft.EntityFrameworkCore;

namespace Canopy.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }


        public DbSet<User> Users { get; set; }
        public DbSet<UserSecurity> UserSecurity { get; set; }
        public DbSet<UserGroup> UserGroup { get; set; }
        public DbSet<ProjectMember> ProjectMember { get; set; }

        public DbSet<Group> Group { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<PlannedTask> PlannedTask { get; set; }

        public DbSet<Chat> Chat { get; set; }
        public DbSet<Message> Message { get; set; }
        public DbSet<MessageAttachment> MessageAttachment { get; set; }
        public DbSet<MessageSeenStatus> MessageSeenStatus { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new UserSecurityConfiguration());
            modelBuilder.ApplyConfiguration(new GroupConfiguration());
            modelBuilder.ApplyConfiguration(new UserGroupConfiguration());
            modelBuilder.ApplyConfiguration(new ProjectConfiguration());
            modelBuilder.ApplyConfiguration(new ProjectMemberConfiguration());
            modelBuilder.ApplyConfiguration(new PlannedTaskConfiguration());
            modelBuilder.ApplyConfiguration(new ChatConfiguration());
            modelBuilder.ApplyConfiguration(new MessageConfiguration());
            modelBuilder.ApplyConfiguration(new MessageAttachmentConfiguration());
            modelBuilder.ApplyConfiguration(new MessageSeenStatusConfiguration());
        }
    }
}
