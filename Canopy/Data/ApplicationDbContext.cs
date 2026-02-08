using Canopy.Data.Configurations;
using Canopy.Models;
using Microsoft.EntityFrameworkCore;

namespace Canopy.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }


        public DbSet<User> Users { get; set; }
        public DbSet<Group> Group { get; set; }
        public DbSet<UserGroup> UserGroup { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // روش ۱: اعمال مستقیم Configuration
            modelBuilder.ApplyConfiguration(new UserConfiguration());
        }
    }
}
