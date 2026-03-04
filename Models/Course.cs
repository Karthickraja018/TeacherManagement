using System.Collections.Generic;

namespace TeacherManagement.Models
{
    public class Course
    {
        public int CourseId { get; set; }

        public string CourseName { get; set; }

        public ICollection<Student> Students { get; set; } = new List<Student>();

        public ICollection<Subject> Subjects { get; set; } = new List<Subject>();
    }
}
