using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TeacherManagement.Models
{
    public class RefreshToken
    {
        public int Id { get; set; }

        [Required]
        public string Token { get; set; }

        [Required]
        public string Username { get; set; }

        public DateTime ExpiresAt { get; set; }

        public bool IsRevoked { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey(nameof(AppUser))]
        public int AppUserId { get; set; }
        public AppUser AppUser { get; set; }
    }
}
