namespace TeacherManagement.DTOs
{
    public class StudentDetailsDto
    {
        public int StudentId { get; set; }
        public string StudentName { get; set; }

        public int? BranchId { get; set; }
        public string BranchName { get; set; }

        public int? AddressId { get; set; }
        public string FullAddress { get; set; }

        public string CourseIdsCsv { get; set; }
        public string CourseNamesCsv { get; set; }
    }
}