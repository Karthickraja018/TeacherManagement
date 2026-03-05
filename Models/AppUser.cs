using System.ComponentModel.DataAnnotations;

namespace TeacherManagement.Models
{
    public class AppUser
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Username { get; set; }

        [Required]
        [MaxLength(200)]
        public string Email { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        [Required]
        [MaxLength(20)]
        public string Role { get; set; }

        /// <summary>
        /// For Teacher role: links to Teachers.TeacherId.
        /// For Student role: links to Students.StudentId.
        /// Null for Admin role.
        /// </summary>
        public int? LinkedEntityId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
