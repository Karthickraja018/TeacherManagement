namespace TeacherManagement.DTOs
{
    public class SubjectDto
    {
        public int SubjectId { get; set; }
        public string Name { get; set; }
        public List<int> CourseIds { get; set; } = new List<int>();
        public List<string> CourseNames { get; set; } = new List<string>();
    }

    public class SubjectCreateDto
    {
        public string Name { get; set; }
        public List<string> CourseNames { get; set; } = new List<string>();
    }

    public class SubjectUpdateDto
    {
        public string Name { get; set; }
        public List<string> CourseNames { get; set; } = new List<string>();
    }
}
