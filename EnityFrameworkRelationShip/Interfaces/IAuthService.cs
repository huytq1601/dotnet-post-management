using EnityFrameworkRelationShip.Dtos.User;
using Microsoft.AspNetCore.Identity;

namespace EnityFrameworkRelationShip.Interfaces
{
    public interface IAuthService
    {
        Task<IdentityResult> Register(RegisterDto registerDto);
        Task<AuthResponseDto?> Login(LoginDto loginDto);
        Task<string> CreateRefreshToken();
        Task<AuthResponseDto?> VerifyRefreshToken(AuthResponseDto authResponseDto);
    }
}
