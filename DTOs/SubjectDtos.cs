using System.ComponentModel.DataAnnotations;

namespace TeacherManagement.DTOs
{
    public class SubjectDto
    {
        public int SubjectId { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; }

        public List<int> CourseIds { get; set; } = new List<int>();
        public List<string> CourseNames { get; set; } = new List<string>();
    }

    public class SubjectCreateDto
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; }

        public List<string> CourseNames { get; set; } = new List<string>();
    }

    public class SubjectUpdateDto
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; }

        public List<string> CourseNames { get; set; } = new List<string>();
    }
}
