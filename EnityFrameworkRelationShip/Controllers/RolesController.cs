using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PostManagement.Application.Dtos.Post;
using PostManagement.Application.Dtos.Role;
using PostManagement.Core.Common;
using PostManagement.Core.Exceptions;
using System.Security.Claims;

namespace PostManagement.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class RolesController : ControllerBase
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMapper _mapper;

        public RolesController(RoleManager<IdentityRole> roleManager, IMapper mapper)
        {
            this._roleManager = roleManager;
            this._mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RoleDto>>> GetAll()
        {
            var roles = await _roleManager.Roles.ToListAsync();

            var roleDtos = new List<RoleDto>();

            foreach (var role in roles)
            {
                var allClaims = await _roleManager.GetClaimsAsync(role);
                var permissions = allClaims.Where(c => c.Type == "Permission").Select(p => p.Value).ToList();
                var roleDto = new RoleDto
                {
                    Id = role.Id,
                    Name = role.Name!,
                    Permissions = permissions
                };

                roleDtos.Add(roleDto);
            }

            return Ok(roleDtos);
        }

        [HttpGet("permissions")]
        public async Task<ActionResult<IEnumerable<string>>> GetAllPermissions()
        {
            await Task.CompletedTask;
            var permissions = Permissions.GetAllPermissions();
            return Ok(permissions);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRole(string id, [FromBody] RoleDto roleDto)
        {
            if (roleDto == null || roleDto.Id != id)
            {
                throw new BadRequestException("Invalid role data");
            }

            var role = await _roleManager.Roles.FirstOrDefaultAsync(r => r.Id == id);
            var allClaims = await _roleManager.GetClaimsAsync(role);
            var permissionClaims = allClaims.Where(c => c.Type == "Permission").ToList();

            var permissionNames = roleDto.Permissions;

            foreach(var permissionClaim in permissionClaims) { 
                if(!roleDto.Permissions.Contains(permissionClaim.Value))
                {
                    await _roleManager.RemoveClaimAsync(role, permissionClaim);
                }
                else
                {
                    roleDto.Permissions.Remove(permissionClaim.Value);
                }
            }

            foreach(var permission in roleDto.Permissions)
            {
                await _roleManager.AddClaimAsync(role, new Claim("Permission", permission));
            }

            return NoContent();
        }
    }
}
