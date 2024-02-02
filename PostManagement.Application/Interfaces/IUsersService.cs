using PostManagement.Application.Dtos.Post;
using PostManagement.Application.Dtos.User;

namespace PostManagement.Application.Interfaces
{
    public interface IUsersService
    {
        Task<IEnumerable<UserDto>> GetAllUsersAsync(string userId);
        Task<UserDto?> GetUserByIdAsync(string id);
        Task<AssignResultDto> AssignRoleAsync(AssignRoleDto assignRoleDto);
        Task<bool> DeleteUserAsync(string userId);
    }
}
