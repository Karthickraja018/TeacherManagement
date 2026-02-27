using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TeacherManagement.Models
{
    public class Subject
    {
        [Key]
        public int SubjectId { get; set; }

        [Required]
        public string Name { get; set; }

        public ICollection<Teacher> Teachers { get; set; } = new List<Teacher>();

        public ICollection<Course> Courses { get; set; } = new List<Course>();
    }
}
