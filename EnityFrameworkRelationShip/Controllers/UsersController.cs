using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PostManagement.Application.Dtos.User;
using PostManagement.Application.Interfaces;
using PostManagement.Core.Common;
using PostManagement.Core.Exceptions;

namespace PostManagement.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUsersService _usersService;

        public UsersController(IUsersService usersService)
        {
            _usersService = usersService;
        }

        [Authorize(Policy = Permissions.Users.CanRead)]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
        {
            var currentUserId = User.Claims.FirstOrDefault(c => c.Type == "uid")!.Value;
            var users = await _usersService.GetAllUsersAsync(currentUserId);
            return Ok(users);
        }

        [Authorize(Roles = "Admin, User")]
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetUser(string id)
        {
            var user = await _usersService.GetUserByIdAsync(id);
            return Ok(user);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateUser(string id, [FromBody] UserDto userDto)
        {
            if (userDto == null || userDto.Id != id)
            {
                throw new BadRequestException("Invalid user data");
            }

            await _usersService.UpdateUserAsync(userDto);

            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteUser(string id)
        {
            var success = await _usersService.DeleteUserAsync(id);

            if (!success)
            {
                return NotFound("User not found");
            }
            else
            {
                return NoContent();
            }
        }
    }
}
