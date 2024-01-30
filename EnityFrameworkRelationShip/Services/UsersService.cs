using AutoMapper;
using EnityFrameworkRelationShip.Dtos.Post;
using EnityFrameworkRelationShip.Dtos.User;
using EnityFrameworkRelationShip.Interfaces;
using EnityFrameworkRelationShip.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EnityFrameworkRelationShip.Services
{
    public class UsersService: IUsersService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMapper _mapper;
        private readonly IRepository<Post> _postRepository;

        public UsersService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IMapper mapper, IRepository<Post> postRepository)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _mapper = mapper;
            _postRepository = postRepository;
        }

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            var users = await _userManager.Users.ToListAsync();
            var userDtos = _mapper.Map<IList<UserDto>>(users);

            for(var i = 0; i < users.Count; i++)
            {
                userDtos[i].Roles = await _userManager.GetRolesAsync(users[i]);
            }

            return userDtos;
        }

        public async Task<UserDto?> GetUserByIdAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if(user == null)
            {
                return null;
            }

            var userDto = _mapper.Map<UserDto>(user);
            userDto.Roles = await _userManager.GetRolesAsync(user);
            return userDto;
        }

        public async Task<IEnumerable<PostWithTagsDto>> GetPostsByUser(string userId)
        {
            var posts = await _postRepository.FindManyAsync(p => p.UserId == userId);
            return _mapper.Map<List<PostWithTagsDto>>(posts);
        }

        public async Task<AssignResultDto> AssignRoleAsync(AssignRoleDto assignRoleDto)
        {
            var result = new AssignResultDto();

            var user = await _userManager.FindByIdAsync(assignRoleDto.UserId);

            if(user == null)
            {
                result.Errors.Add("User not found.");
                return result;
            }

            var role = await _roleManager.FindByIdAsync(assignRoleDto.RoleId);

            if (role == null)
            {
                result.Errors.Add("Role not found.");
                return result;
            }

            var userRoles = await _userManager.GetRolesAsync(user);

            if (userRoles.Contains(role.Name!))
            {
                result.Errors.Add("User already assigned to the role.");
                return result;
            }

            var identityResult = await _userManager.AddToRoleAsync(user, role.Name!);

            if (!identityResult.Succeeded)
            {
                result.Errors.AddRange(identityResult.Errors.Select(e => e.Description));
                return result;
            }

            result.Success = true;
            return result;
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
