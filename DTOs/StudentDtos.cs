namespace TeacherManagement.DTOs
{
    public class StudentDto
    {
        public int StudentId { get; set; }
        public string Name { get; set; }
        public int BranchId { get; set; }
        public BranchDto? Branch { get; set; }
        public int? AddressId { get; set; }
        public AddressDto? Address { get; set; }
        public List<int> CourseIds { get; set; } = new List<int>();
    }

    public class StudentCreateDto
    {
        public string Name { get; set; }
        public string BranchName { get; set; }
        public AddressCreateDto? Address { get; set; }
        public List<string> CourseNames { get; set; } = new List<string>();
    }

    public class StudentUpdateDto
    {
        public string Name { get; set; }
        public string BranchName { get; set; }
        public AddressCreateDto? Address { get; set; }
        public List<string> CourseNames { get; set; } = new List<string>();
    }
}
