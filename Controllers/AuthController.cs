using Microsoft.AspNetCore.Mvc;
using TeacherManagement.DTOs;
using TeacherManagement.Services;

namespace TeacherManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost]
        public async Task<ActionResult<AuthResponseDto>> Register(RegisterDto model)
        {
            var result = await _authService.RegisterAsync(model);
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<AuthResponseDto>> Login(LoginDto model)
        {
            var result = await _authService.LoginAsync(model);
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<AuthResponseDto>> RefreshToken(RefreshTokenRequestDto model)
        {
            var result = await _authService.RefreshAsync(model);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> RevokeToken(RefreshTokenRequestDto model)
        {
            var revoked = await _authService.RevokeAsync(model);
            if (!revoked)
                return NotFound("Refresh token not found.");

            return Ok("Token revoked successfully.");
        }
    }
}
