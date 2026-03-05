using System.ComponentModel.DataAnnotations;

namespace TeacherManagement.DTOs
{
    public class TeacherDto
    {
        public int TeacherId { get; set; }

        [Required]
        [MaxLength(200)]
        public string TeacherName { get; set; }

        [Required]
        public int BranchId { get; set; }
        public BranchDto? Branch { get; set; }
        public int? AddressId { get; set; }
        public AddressDto? Address { get; set; }
        public List<int> SubjectIds { get; set; } = new List<int>();
    }

    public class TeacherCreateDto
    {
        [Required]
        [MaxLength(200)]
        public string TeacherName { get; set; }

        [Required]
        [MaxLength(200)]
        public string BranchName { get; set; }

        public AddressCreateDto? Address { get; set; }

        public List<string> SubjectNames { get; set; } = new List<string>();

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
    }

    public class TeacherUpdateDto
    {
        [Required]
        [MaxLength(200)]
        public string TeacherName { get; set; }

        [Required]
        [MaxLength(200)]
        public string BranchName { get; set; }

        public AddressCreateDto? Address { get; set; }

        public List<string> SubjectNames { get; set; } = new List<string>();
    }
}
