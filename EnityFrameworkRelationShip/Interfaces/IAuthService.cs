using EnityFrameworkRelationShip.Dtos.User;
using Microsoft.AspNetCore.Identity;

namespace EnityFrameworkRelationShip.Interfaces
{
    public interface IAuthService
    {
        Task<IdentityResult> Register(RegisterDto registerDto);
    }
}
