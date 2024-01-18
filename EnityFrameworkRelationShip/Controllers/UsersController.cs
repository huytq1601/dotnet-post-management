using EnityFrameworkRelationShip.Dtos.Post;
using EnityFrameworkRelationShip.Dtos.User;
using EnityFrameworkRelationShip.Interfaces.Service;
using EnityFrameworkRelationShip.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EnityFrameworkRelationShip.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUsersService _usersService;

        public UsersController(IUsersService usersService)
        {
            _usersService = usersService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<ApplicationUser>>> GetUsers()
        {
            var users = await _usersService.GetAllUsersAsync();
            return Ok(users);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> AssignRole(string id, [FromBody] AssignRoleDto assignRoleDto)
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
                // You may want to return the validation problem details
                return BadRequest(ModelState);
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
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
