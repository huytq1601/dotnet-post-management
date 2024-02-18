using Microsoft.AspNetCore.Identity;
using PostManagement.Core.Common;
using PostManagement.Core.Entities;
using System.Security.Claims;
using System.Security;
using System.Runtime.ConstrainedExecution;

namespace PostManagement.Infrastructure.Data
{
    public class DatabaseSeeder
    {
        public static async Task SeedDatabaseAsync(DataContext context, UserManager<ApplicationUser> userManager)
        {
            if (!context.Posts.Any())
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
                await userManager.AddToRoleAsync(user, "Admin");

                var posts = new List<Post>()
                {
                    new Post
                    {

                        Title = "First Post",
                        Content = "Hello World!",
                        DatePublished = DateTime.Now,
                        User = user,
                        Tags = new List<Tag>()
                        {
                            new Tag {Name = "Science"}
                        }
                    },
                     new Post
                     {
                        Title = "EF Core",
                        Content = "Seeding data with EF Core.",
                        DatePublished = DateTime.Now,
                        User = user,
                        Tags = new List<Tag>
                        {
                            new Tag {Name = "Tech"}
                        }
                     }
                };
                await context.Posts.AddRangeAsync(posts);

                var tags = new List<Tag>
                {
                    new Tag { Name = "Art" },
                    new Tag { Name = "Travel"},
                    new Tag { Name = "Study"}
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

                    if (roleName == "Admin")
                    {
                        var permissions = Permissions.GetAllPermissions();
                        foreach (var permission in permissions)
                        {
                            await roleManager.AddClaimAsync(role, new Claim("Permission", permission));
                        }
                    }

                    if (roleName == "User")
                    {
                        var permissions = Permissions.GetPermissionsByClass("Posts");
                        foreach (var permission in permissions)
                        {
                            await roleManager.AddClaimAsync(role, new Claim("Permission", permission));
                        }
                    }
                }
            }
        }
    }
}
