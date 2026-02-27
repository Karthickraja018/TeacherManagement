using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TeacherManagement.Models
{
    public class Course
    {
        [Key]
        public int CourseId { get; set; }

        [Required]
        public string CourseName { get; set; }

        public ICollection<Student> Students { get; set; } = new List<Student>();

        public ICollection<Subject> Subjects { get; set; } = new List<Subject>();
    }
}
