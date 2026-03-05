using System.ComponentModel.DataAnnotations;

namespace TeacherManagement.DTOs
{
    public class CourseDto
    {
        public int CourseId { get; set; }

        [Required]
        [MaxLength(200)]
        public string CourseName { get; set; }

        public List<int> SubjectIds { get; set; } = new List<int>();
        public List<string> SubjectNames { get; set; } = new List<string>();
    }

    public class CourseCreateDto
    {
        [Required]
        [MaxLength(200)]
        public string CourseName { get; set; }

        public List<string> SubjectNames { get; set; } = new List<string>();
    }

    public class CourseUpdateDto
    {
        [Required]
        [MaxLength(200)]
        public string CourseName { get; set; }

        public List<string> SubjectNames { get; set; } = new List<string>();
    }
}
