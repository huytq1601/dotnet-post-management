using EnityFrameworkRelationShip.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EnityFrameworkRelationShip.Data
{
    public class DatabaseSeeder
    {
        public static async Task SeedDatabaseAsync(DataContext context, UserManager<ApplicationUser> userManager)
        {
            if (!context.PostTags.Any())
            {
                var user = new ApplicationUser
                {
                    FirstName = "Huy",
                    LastName = "Ta",
                    Email = "huy.ta@example.com",
                    UserName = "huy.ta"
                };

                await userManager.CreateAsync(user, "Password@123");
                await userManager.AddToRoleAsync(user, "User");
                var postTags = new List<PostTag>()
                {
                    new PostTag
                    {
                        Post = new Post
                        {
                            Title = "First Post",
                            Content = "Hello World!",
                            DatePublished = DateTime.Now,
                            User = user
                        },
                        Tag = new Tag {Name = "Science"}
                    },
                     new PostTag
                     {
                        Post = new Post
                        {
                            Title = "EF Core",
                            Content = "Seeding data with EF Core.",
                            DatePublished = DateTime.Now,
                            User = user
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

        public static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            string[] roleNames = { "Admin", "User", "Operator" };

            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    IdentityRole role = new IdentityRole(roleName);
                    role.NormalizedName = roleName.ToUpper();
                    var roleResult = await roleManager.CreateAsync(role);

                    if (!roleResult.Succeeded)
                    {
                        throw new Exception("Failed to create role: " + roleName);
                    }
                }
            }
        }
    }
}
