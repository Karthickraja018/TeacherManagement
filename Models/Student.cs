using System.Collections.Generic;


namespace TeacherManagement.Models
{
    public class Student
    {
        public int StudentId { get; set; }

        public string Name { get; set; }

        public int BranchId { get; set; }
        public Branch Branch { get; set; }

        public int? AddressId { get; set; }
        public Address Address { get; set; }

        public ICollection<Course> Courses { get; set; } = new List<Course>();
    }
}
