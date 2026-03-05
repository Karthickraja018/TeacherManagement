using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using TeacherManagement.Data;
using TeacherManagement.Models;

namespace TeacherManagement.Services
{
    public class TokenService
    {
        private readonly TeacherContext _db;
        private readonly IConfiguration _config;

        public TokenService(TeacherContext db, IConfiguration config)
        {
            _db = db;
            _config = config;
        }

        public string GenerateRefreshToken()
        {
            var randomBytes = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes);
        }

        public async Task SaveRefreshTokenAsync(AppUser user, string refreshToken)
        {
            var expiryDays = int.Parse(_config["JwtSettings:RefreshTokenExpiryDays"]!);

            var token = new RefreshToken
            {
                Token = refreshToken,
                Username = user.Username,
                AppUserId = user.Id,
                ExpiresAt = DateTime.UtcNow.AddDays(expiryDays),
                IsRevoked = false,
                CreatedAt = DateTime.UtcNow
            };

            _db.RefreshTokens.Add(token);
            await _db.SaveChangesAsync();
        }

        public async Task<AppUser?> RetrieveUserByRefreshTokenAsync(string refreshToken)
        {
            var tokenEntry = await _db.RefreshTokens
                .Include(r => r.AppUser)
                .FirstOrDefaultAsync(r => r.Token == refreshToken);

            if (tokenEntry == null || tokenEntry.IsRevoked || tokenEntry.ExpiresAt <= DateTime.UtcNow)
                return null;

            return tokenEntry.AppUser;
        }

        public async Task<bool> RevokeRefreshTokenAsync(string refreshToken)
        {
            var tokenEntry = await _db.RefreshTokens.FirstOrDefaultAsync(r => r.Token == refreshToken);
            if (tokenEntry == null)
                return false;

            tokenEntry.IsRevoked = true;
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
