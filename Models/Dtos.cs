using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TeacherManagement.Models
{
    public class StudentDto
    {
        public int StudentId { get; set; }
        public string Name { get; set; }
        public int BranchId { get; set; }
        public BranchDto? Branch { get; set; }
        public int? AddressId { get; set; }
        public AddressDto? Address { get; set; }
        public List<int> CourseIds { get; set; } = new List<int>();
    }

    public class StudentCreateDto
    {
        [Required]
        [StringLength(200)]
        public string Name { get; set; }

        [Required]
        [StringLength(200)]
        public string BranchName { get; set; }

        // Optional address object instead of AddressId
        public AddressCreateDto? Address { get; set; }

        // Client supplies course names instead of IDs
        public List<string> CourseNames { get; set; } = new List<string>();
    }

    public class StudentUpdateDto
    {
        [Required]
        [StringLength(200)]
        public string Name { get; set; }

        [Required]
        [StringLength(200)]
        public string BranchName { get; set; }

        public AddressCreateDto? Address { get; set; }

        public List<string> CourseNames { get; set; } = new List<string>();
    }

    public class TeacherDto
    {
        public int TeacherId { get; set; }
        public string TeacherName { get; set; }
        public int BranchId { get; set; }
        public BranchDto? Branch { get; set; }
        public int? AddressId { get; set; }
        public AddressDto? Address { get; set; }
        public List<int> SubjectIds { get; set; } = new List<int>();
    }

    public class TeacherCreateDto
    {
        [Required]
        [StringLength(200)]
        public string TeacherName { get; set; }

        [Required]
        [StringLength(200)]
        public string BranchName { get; set; }

        public AddressCreateDto? Address { get; set; }

        // Client supplies subject names instead of IDs
        public List<string> SubjectNames { get; set; } = new List<string>();
    }

    public class TeacherUpdateDto
    {
        [Required]
        [StringLength(200)]
        public string TeacherName { get; set; }

        [Required]
        [StringLength(200)]
        public string BranchName { get; set; }

        public AddressCreateDto? Address { get; set; }

        public List<string> SubjectNames { get; set; } = new List<string>();
    }

    public class BranchDto
    {
        public int BranchId { get; set; }
        public string Name { get; set; }
    }

    public class AddressDto
    {
        public int AddressId { get; set; }
        public string Line1 { get; set; }
        public string? Line2 { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Zip { get; set; }
    }

    public class AddressCreateDto
    {
        [Required]
        [StringLength(200)]
        public string Line1 { get; set; }

        [StringLength(200)]
        public string? Line2 { get; set; }

        [StringLength(100)]
        public string? City { get; set; }

        [StringLength(100)]
        public string? State { get; set; }

        [StringLength(20)]
        public string? Zip { get; set; }
    }

    // --- Course and Subject DTOs ---

    public class CourseDto
    {
        public int CourseId { get; set; }
        public string CourseName { get; set; }
        public List<int> SubjectIds { get; set; } = new List<int>();
        public List<string> SubjectNames { get; set; } = new List<string>();
    }

    public class CourseCreateDto
    {
        [Required]
        [StringLength(200)]
        public string CourseName { get; set; }

        // Client can supply subject names to associate
        public List<string> SubjectNames { get; set; } = new List<string>();
    }

    public class CourseUpdateDto
    {
        [Required]
        [StringLength(200)]
        public string CourseName { get; set; }

        public List<string> SubjectNames { get; set; } = new List<string>();
    }

    public class SubjectDto
    {
        public int SubjectId { get; set; }
        public string Name { get; set; }
        public List<int> CourseIds { get; set; } = new List<int>();
        public List<string> CourseNames { get; set; } = new List<string>();
    }

    public class SubjectCreateDto
    {
        [Required]
        [StringLength(200)]
        public string Name { get; set; }

        // Client can supply course names to associate
        public List<string> CourseNames { get; set; } = new List<string>();
    }

    public class SubjectUpdateDto
    {
        [Required]
        [StringLength(200)]
        public string Name { get; set; }

        public List<string> CourseNames { get; set; } = new List<string>();
    }
}
