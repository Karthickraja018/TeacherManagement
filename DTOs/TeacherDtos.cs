namespace TeacherManagement.DTOs
{
    public class TeacherDto
    {
        public int TeacherId { get; set; }
        public string TeacherName { get; set; }
        public int BranchId { get; set; }
        public BranchDto? Branch { get; set; }
        public int? AddressId { get; set; }
        public AddressDto? Address { get; set; }
        public List<int> SubjectIds { get; set; } = new List<int>();
    }

    public class TeacherCreateDto
    {
        public string TeacherName { get; set; }
        public string BranchName { get; set; }
        public AddressCreateDto? Address { get; set; }
        public List<string> SubjectNames { get; set; } = new List<string>();
    }

    public class TeacherUpdateDto
    {
        public string TeacherName { get; set; }
        public string BranchName { get; set; }
        public AddressCreateDto? Address { get; set; }
        public List<string> SubjectNames { get; set; } = new List<string>();
    }
}
