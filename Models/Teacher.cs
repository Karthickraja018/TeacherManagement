using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TeacherManagement.Models
{
    public class Teacher
    {
        [Key]
        public int TeacherId { get; set; }

        [Required]
        public string TeacherName { get; set; }

        public int BranchId { get; set; }
        public Branch Branch { get; set; }

        public int? AddressId { get; set; }
        public Address Address { get; set; }

        public ICollection<Subject> Subjects { get; set; } = new List<Subject>();
    }
}
