using TeacherManagement.DTOs;

namespace TeacherManagement.Services
{
    public interface IAuthService
    {
        Task<AuthResponseDto> RegisterAsync(RegisterDto model);
        Task<AuthResponseDto> LoginAsync(LoginDto model);
        Task<AuthResponseDto> RefreshAsync(RefreshTokenRequestDto model);
        Task<bool> RevokeAsync(RefreshTokenRequestDto model);
    }
}
