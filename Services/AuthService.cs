using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TeacherManagement.Data;
using TeacherManagement.DTOs;
using TeacherManagement.Models;

namespace TeacherManagement.Services
{
    public class AuthService : IAuthService
    {
        private readonly TeacherContext _db;
        private readonly IConfiguration _config;
        private readonly TokenService _tokenService;

        public AuthService(TeacherContext db, IConfiguration config, TokenService tokenService)
        {
            _db = db;
            _config = config;
            _tokenService = tokenService;
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterDto model)
        {
            var emailExists = await _db.AppUsers.AnyAsync(u => u.Email.ToLower() == model.Email.ToLower());
            if (emailExists)
                throw new InvalidOperationException("A user with this email already exists.");

            var usernameExists = await _db.AppUsers.AnyAsync(u => u.Username.ToLower() == model.Username.ToLower());
            if (usernameExists)
                throw new InvalidOperationException("A user with this username already exists.");

            if (model.Role == "Teacher" && model.LinkedEntityId.HasValue)
            {
                var teacherExists = await _db.Teachers.AnyAsync(t => t.TeacherId == model.LinkedEntityId.Value);
                if (!teacherExists)
                    throw new InvalidOperationException($"Teacher with id {model.LinkedEntityId.Value} does not exist.");
            }

            if (model.Role == "Student" && model.LinkedEntityId.HasValue)
            {
                var studentExists = await _db.Students.AnyAsync(s => s.StudentId == model.LinkedEntityId.Value);
                if (!studentExists)
                    throw new InvalidOperationException($"Student with id {model.LinkedEntityId.Value} does not exist.");
            }

            var user = new AppUser
            {
                Username = model.Username.Trim(),
                Email = model.Email.Trim().ToLower(),
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password),
                Role = model.Role,
                LinkedEntityId = model.LinkedEntityId
            };

            _db.AppUsers.Add(user);
            await _db.SaveChangesAsync();

            return await BuildAuthResponseAsync(user);
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto model)
        {
            var user = await _db.AppUsers.FirstOrDefaultAsync(u => u.Email.ToLower() == model.Email.ToLower());
            if (user == null || !BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash))
                throw new UnauthorizedAccessException("Invalid email or password.");

            return await BuildAuthResponseAsync(user);
        }

        public async Task<AuthResponseDto> RefreshAsync(RefreshTokenRequestDto model)
        {
            var user = await _tokenService.RetrieveUserByRefreshTokenAsync(model.RefreshToken);
            if (user == null)
                throw new UnauthorizedAccessException("Invalid or expired refresh token.");

            await _tokenService.RevokeRefreshTokenAsync(model.RefreshToken);

            return await BuildAuthResponseAsync(user);
        }

        public async Task<bool> RevokeAsync(RefreshTokenRequestDto model)
        {
            return await _tokenService.RevokeRefreshTokenAsync(model.RefreshToken);
        }

        private async Task<AuthResponseDto> BuildAuthResponseAsync(AppUser user)
        {
            var (accessToken, expiry) = GenerateAccessToken(user);
            var refreshToken = _tokenService.GenerateRefreshToken();
            await _tokenService.SaveRefreshTokenAsync(user, refreshToken);

            return new AuthResponseDto
            {
                Token = accessToken,
                RefreshToken = refreshToken,
                Username = user.Username,
                Email = user.Email,
                Role = user.Role,
                LinkedEntityId = user.LinkedEntityId,
                ExpiresAt = expiry
            };
        }

        private (string token, DateTime expiry) GenerateAccessToken(AppUser user)
        {
            var jwtSettings = _config.GetSection("JwtSettings");
            var secretKey = jwtSettings["SecretKey"]!;
            var issuer = jwtSettings["Issuer"]!;
            var audience = jwtSettings["Audience"]!;
            var expiryMinutes = int.Parse(jwtSettings["ExpiryMinutes"]!);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiry = DateTime.UtcNow.AddMinutes(expiryMinutes);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            if (user.LinkedEntityId.HasValue)
                claims.Add(new Claim("linked_entity_id", user.LinkedEntityId.Value.ToString()));

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: expiry,
                signingCredentials: credentials
            );

            return (new JwtSecurityTokenHandler().WriteToken(token), expiry);
        }
    }
}
