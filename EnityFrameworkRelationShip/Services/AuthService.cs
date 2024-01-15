using EnityFrameworkRelationShip.Dtos.User;
using EnityFrameworkRelationShip.Interfaces;
using EnityFrameworkRelationShip.Models;
using Microsoft.AspNetCore.Identity;

namespace EnityFrameworkRelationShip.Services
{
    public class AuthService: IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public AuthService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IdentityResult> Register(RegisterDto registerDto)
        {
            var user = new ApplicationUser
            {
                UserName = registerDto.Username,
                Email = registerDto.Email,
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);

            if (result.Succeeded)
            {
                await EnsureRoleExists("User");

                await _userManager.AddToRoleAsync(user, "User");
            }

            return result;

        }

        private async Task EnsureRoleExists(string roleName)
        {
            var roleExists = await _roleManager.RoleExistsAsync(roleName);
            if (!roleExists)
            {
                var newRole = new IdentityRole(roleName);
                newRole.NormalizedName = roleName.ToUpper();
                await _roleManager.CreateAsync(newRole);
            }
        }
    }
}
