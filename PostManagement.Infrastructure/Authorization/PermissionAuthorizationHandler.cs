using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PostManagement.Core.Entities;
using PostManagement.Core.Exceptions;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace PostManagement.Infrastructure.Authorization
{
    public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public PermissionAuthorizationHandler(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            var id = context.User.Claims.FirstOrDefault(c => c.Type == "uid")?.Value;
            var user = await _userManager.FindByIdAsync(id);
            var roleNames = await _userManager.GetRolesAsync(user);

            var permissions = new HashSet<string>();

            foreach (var roleName in roleNames)
            {
                var role = await _roleManager.Roles.FirstAsync(r => r.Name == roleName);
                var allClaims = await _roleManager.GetClaimsAsync(role);
                var allPermissions = allClaims.Where(c => c.Type == "Permission").Select(p => p.Value);
                permissions = new HashSet<string>(permissions.Concat(allPermissions));
            }

            if(permissions.Contains(requirement.Permission))
            {
                context.Succeed(requirement);
            }
        }
    }
}
