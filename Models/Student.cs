using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace TeacherManagement.Models
{
    public class Student
    {
        [Required]
        public int StudentId { get; set; }

        [Required]
        public string Name { get; set; }

        public int BranchId { get; set; }
        public Branch Branch { get; set; }

        public int? AddressId { get; set; }
        public Address Address { get; set; }

        public ICollection<Course> Courses { get; set; } = new List<Course>();
    }
}
