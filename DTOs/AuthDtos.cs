using System.ComponentModel.DataAnnotations;

namespace TeacherManagement.DTOs
{
    public class RegisterDto
    {
        [Required]
        [MaxLength(100)]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(200)]
        public string Email { get; set; }

        [Required]
        [MinLength(6)]
        public string Password { get; set; }

        [Required]
        [RegularExpression("^(Admin|Teacher|Student)$", ErrorMessage = "Role must be Admin, Teacher or Student.")]
        public string Role { get; set; }

        /// <summary>
        /// Optional: the TeacherId or StudentId this account belongs to.
        /// Required when Role is Teacher or Student.
        /// </summary>
        public int? LinkedEntityId { get; set; }
    }

    public class LoginDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }

    public class AuthResponseDto
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public int? LinkedEntityId { get; set; }
        public DateTime ExpiresAt { get; set; }
    }

    public class RefreshTokenRequestDto
    {
        [Required]
        public string RefreshToken { get; set; }
    }
}
