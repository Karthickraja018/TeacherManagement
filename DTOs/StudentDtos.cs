using System.ComponentModel.DataAnnotations;

namespace TeacherManagement.DTOs
{
    public class StudentDto
    {
        public int StudentId { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; }

        [Required]
        public int BranchId { get; set; }
        public BranchDto? Branch { get; set; }
        public int? AddressId { get; set; }
        public AddressDto? Address { get; set; }
        public List<int> CourseIds { get; set; } = new List<int>();
    }

    public class StudentCreateDto
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; }

        [Required]
        [MaxLength(200)]
        public string BranchName { get; set; }

        public AddressCreateDto? Address { get; set; }

        public List<string> CourseNames { get; set; } = new List<string>();
    }

    public class StudentUpdateDto
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; }

        [Required]
        [MaxLength(200)]
        public string BranchName { get; set; }

        public AddressCreateDto? Address { get; set; }

        public List<string> CourseNames { get; set; } = new List<string>();
    }
}
