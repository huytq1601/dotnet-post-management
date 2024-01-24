using EnityFrameworkRelationShip.Dtos.Post;
using EnityFrameworkRelationShip.Dtos.User;
using EnityFrameworkRelationShip.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EnityFrameworkRelationShip.Controllers
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

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
        {
            var users = await _usersService.GetAllUsersAsync();
            return Ok(users);
        }

        [Authorize(Roles = "Admin, User")]
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>>GetUser(string id)
        {
            var user = await _usersService.GetUserByIdAsync(id);
            return Ok(user);
        }

        [Authorize]
        [HttpGet("{id}/posts")]
        public async Task<ActionResult<PostWithTagsDto>> GetUserPosts(string id)
        {
            var posts = await _usersService.GetPostsByUser(id);
            return Ok(posts);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<ActionResult> AssignRoles(string id, [FromBody] AssignRoleDto assignRoleDto)
        {
            if(assignRoleDto == null || assignRoleDto.UserId != id)
            {
                return BadRequest("Invalid user data");
            }

            var result = await _usersService.AssignRoleAsync(assignRoleDto);

            if (!result.Success)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("errors", error);
                }

                return BadRequest(ModelState);
            }

            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteUser(string id)
        {
            var success = await _usersService.DeleteUserAsync(id);

            if(!success)
            {
                return NotFound("User not found");
            } else
            {
                return NoContent();
            }
        }
    }
}
