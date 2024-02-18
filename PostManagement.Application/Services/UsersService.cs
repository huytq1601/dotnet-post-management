using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PostManagement.Application.Dtos.User;
using PostManagement.Application.Interfaces;
using PostManagement.Core.Entities;
using PostManagement.Core.Exceptions;
using PostManagement.Core.Interfaces;

namespace PostManagement.Application.Services
{
    public class UsersService : IUsersService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMapper _mapper;

        public UsersService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IMapper mapper, IRepository<Post> postRepository)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _mapper = mapper;
        }

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync(string userId)
        {
            var users = await _userManager.Users.Where(u => u.Id != userId).ToListAsync();
            var userDtos = _mapper.Map<IList<UserDto>>(users);

            for (var i = 0; i < users.Count; i++)
            {
                userDtos[i].Roles = await _userManager.GetRolesAsync(users[i]);
            }

            return userDtos;
        }

        public async Task<UserWithPermissionsDto?> GetUserByIdAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return null;
            }

            var userDto = _mapper.Map<UserWithPermissionsDto>(user);

            userDto.Roles = await _userManager.GetRolesAsync(user);
            var permissions = new HashSet<string>();

            foreach (var roleName in userDto.Roles)
            {
                var role = await _roleManager.FindByNameAsync(roleName);
                var allClaims = await _roleManager.GetClaimsAsync(role);
                var rolePermissions = allClaims.Where(c => c.Type == "Permission").Select(c => c.Value).ToList();
                permissions.UnionWith(rolePermissions);
            }

            userDto.Permissions = permissions;

            return userDto;
        }

        public async Task UpdateUserAsync(UserDto userDto)
        {

            var user = await _userManager.FindByIdAsync(userDto.Id);

            if (user == null)
            {
                throw new NotFoundException("User not found.");
            }

            user.FirstName = userDto.FirstName; 
            user.LastName = userDto.LastName;
            user.Email = userDto.Email;
            user.PhoneNumber = userDto.PhoneNumber;
            user.UserName = userDto.UserName;

            var userRoles = await _userManager.GetRolesAsync(user);

            var rolesToDelete = userRoles.Except(userDto.Roles).ToList();
            var rolesToAdd = userDto.Roles.Except(userRoles).ToList();


            foreach (var role in rolesToDelete)
            {
                await _userManager.RemoveFromRoleAsync(user, role);
            }

            foreach(var role in rolesToAdd)
            {
                await _userManager.AddToRoleAsync(user, role);
            }

            await _userManager.UpdateAsync(user);
        }

        public async Task<bool> DeleteUserAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return false;
            }

            user.IsDeleted = true;
            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }
    }
}
