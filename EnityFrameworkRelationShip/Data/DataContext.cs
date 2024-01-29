using EnityFrameworkRelationShip.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EnityFrameworkRelationShip.Data
{
    public class DataContext : IdentityDbContext<ApplicationUser>
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options){ }

        public DbSet<Post> Posts { get; set; }
        public DbSet<Tag> Tags { get; set; }

        // Configure the many-to-many relationship
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Post>().HasQueryFilter(p => !p.IsDeleted);
            modelBuilder.Entity<ApplicationUser>().HasQueryFilter(u => !u.IsDeleted);

            modelBuilder.Entity<Post>().Navigation(p => p.Tags).AutoInclude();
        }
    }
}
