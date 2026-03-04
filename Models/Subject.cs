using System.Collections.Generic;

namespace TeacherManagement.Models
{
    public class Subject
    {
        public int SubjectId { get; set; }

        public string Name { get; set; }

        public ICollection<Teacher> Teachers { get; set; } = new List<Teacher>();

        public ICollection<Course> Courses { get; set; } = new List<Course>();
    }
}
