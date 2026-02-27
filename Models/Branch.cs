using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TeacherManagement.Models
{
    public class Branch
    {
        [Key]
        public int BranchId { get; set; }

        [Required]
        public string Name { get; set; }

        public ICollection<Student> Students { get; set; } = new List<Student>();

        public ICollection<Teacher> Teachers { get; set; } = new List<Teacher>();
    }
}
