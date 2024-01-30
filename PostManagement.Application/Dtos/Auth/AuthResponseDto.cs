namespace PostManagement.Application.Dtos.Auth
{
    public class AuthResponseDto
    {
        public string UserId { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;

        public string RefreshToken { get; set; } = string.Empty;
    }
}
