using EnityFrameworkRelationShip.Models;
using Microsoft.EntityFrameworkCore;

namespace EnityFrameworkRelationShip.Data
{
    public class DatabaseSeeder
    {
        public static async Task SeedDatabaseAsync(DataContext context)
        {
            if (!context.PostTags.Any())
            {
                var postTags = new List<PostTag>()
                {
                    new PostTag
                    {
                        Post = new Post
                        {
                            Title = "First Post",
                            Content = "Hello World!",
                            DatePublished = DateTime.Now
                        },
                        Tag = new Tag {Name = "Science"}
                    },
                     new PostTag
                     {
                        Post = new Post
                        {
                            Title = "EF Core", 
                            Content = "Seeding data with EF Core.", 
                            DatePublished = DateTime.Now
                        },
                        Tag = new Tag {Name = "Tech"}
                     }
                };
                await context.PostTags.AddRangeAsync(postTags);

                var tags = new List<Tag>
                {
                    new Tag { Name = "Art" },
                    new Tag { Name = "Travel"},
                    new Tag { Name = "Asia"}
                };
                await context.Tags.AddRangeAsync(tags);
            }

            await context.SaveChangesAsync();
        }
    }
}
