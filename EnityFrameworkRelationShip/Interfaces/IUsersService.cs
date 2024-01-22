using EnityFrameworkRelationShip.Dtos.User;
using EnityFrameworkRelationShip.Models;

namespace EnityFrameworkRelationShip.Interfaces
{
    public interface IUsersService
    {
        Task<IEnumerable<UserDto>> GetAllUsersAsync();
        Task<AssignResultDto> AssignRoleAsync(AssignRoleDto assignRoleDto);

        Task<bool> DeleteUserAsync(string userId);
    }
}
