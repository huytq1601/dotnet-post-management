using Microsoft.AspNetCore.Identity;
using PostManagement.Application.Dtos.Auth;

namespace PostManagement.Application.Interfaces
{
    public interface IAuthService
    {
        Task<IdentityResult> Register(RegisterDto registerDto);
        Task<AuthResponseDto?> Login(LoginDto loginDto);
        Task<string> CreateRefreshToken();
        Task<AuthResponseDto?> VerifyRefreshToken(AuthResponseDto authResponseDto);
    }
}
