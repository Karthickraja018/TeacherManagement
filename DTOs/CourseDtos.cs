namespace TeacherManagement.DTOs
{
    public class CourseDto
    {
        public int CourseId { get; set; }
        public string CourseName { get; set; }
        public List<int> SubjectIds { get; set; } = new List<int>();
        public List<string> SubjectNames { get; set; } = new List<string>();
    }

    public class CourseCreateDto
    {
        public string CourseName { get; set; }
        public List<string> SubjectNames { get; set; } = new List<string>();
    }

    public class CourseUpdateDto
    {
        public string CourseName { get; set; }
        public List<string> SubjectNames { get; set; } = new List<string>();
    }
}
