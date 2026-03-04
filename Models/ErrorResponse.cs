namespace TeacherManagement.Models
{
    public class ErrorResponse
    {
        public int StatusCode { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Detail { get; set; } = string.Empty;
        public string? Instance { get; set; }
        public string? TraceId { get; set; }
        public string? StackTrace { get; set; }
        public string? InnerException { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
