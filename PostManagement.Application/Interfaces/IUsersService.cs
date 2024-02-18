using PostManagement.Application.Dtos.Post;
using PostManagement.Application.Dtos.User;

namespace PostManagement.Application.Interfaces
{
    public interface IUsersService
    {
        Task<IEnumerable<UserDto>> GetAllUsersAsync(string userId);
        Task<UserWithPermissionsDto?> GetUserByIdAsync(string id);
        Task UpdateUserAsync(UserDto userDto);
        Task<bool> DeleteUserAsync(string userId);
    }
}
