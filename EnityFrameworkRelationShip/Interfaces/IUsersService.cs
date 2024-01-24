using EnityFrameworkRelationShip.Dtos.Post;
using EnityFrameworkRelationShip.Dtos.User;

namespace EnityFrameworkRelationShip.Interfaces
{
    public interface IUsersService
    {
        Task<IEnumerable<UserDto>> GetAllUsersAsync();
        Task<UserDto?> GetUserByIdAsync(string id);
        Task<IEnumerable<PostWithTagsDto>> GetPostsByUser(string userId);
        Task<AssignResultDto> AssignRoleAsync(AssignRoleDto assignRoleDto);
        Task<bool> DeleteUserAsync(string userId);
    }
}
