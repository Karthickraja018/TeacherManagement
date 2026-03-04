namespace TeacherManagement.DTOs
{
    public class TeacherDetailsDto
    {
        public int TeacherId { get; set; }
        public string TeacherName { get; set; }

        public int? BranchId { get; set; }
        public string BranchName { get; set; }

        public int? AddressId { get; set; }
        public string FullAddress { get; set; }

        public string SubjectIdsCsv { get; set; }
        public string SubjectNamesCsv { get; set; }
    }
}