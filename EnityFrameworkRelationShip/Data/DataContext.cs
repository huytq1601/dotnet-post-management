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
        public DbSet<PostTag> PostTags { get; set; }

        // Configure the many-to-many relationship
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Post>().HasQueryFilter(p => !p.IsDeleted);
            modelBuilder.Entity<Tag>().HasQueryFilter(t => !t.IsDeleted);
            modelBuilder.Entity<ApplicationUser>().HasQueryFilter(u => !u.IsDeleted);
            modelBuilder.Entity<Post>().Navigation(p => p.PostTags).AutoInclude();
            modelBuilder.Entity<PostTag>().Navigation(p => p.Tag).AutoInclude();

            // Configure the many-to-many join entity
            modelBuilder.Entity<PostTag>()
                .HasKey(pt => new { pt.PostId, pt.TagId });

            modelBuilder.Entity<PostTag>()
                .HasOne(pt => pt.Post)
                .WithMany(p => p.PostTags)
                .HasForeignKey(pt => pt.PostId);

            modelBuilder.Entity<PostTag>()
                .HasOne(pt => pt.Tag)
                .WithMany(t => t.PostTags)
                .HasForeignKey(pt => pt.TagId);
        }
    }
}
