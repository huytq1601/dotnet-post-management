﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PostManagement.Application.Dtos.User;
using PostManagement.Application.Interfaces;

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

        [Authorize(Roles = "Admin")]
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
        public async Task<ActionResult> AssignRoles(string id, [FromBody] AssignRoleDto assignRoleDto)
        {
            if (assignRoleDto == null || assignRoleDto.UserId != id)
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
