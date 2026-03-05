using TeacherManagement.DTOs;

namespace TeacherManagement.Services.Interfaces
{
    public interface IStudentService
    {
        Task<PaginationResponse<StudentDetailsDto>> GetAllAsync(PaginationParams parameters);
        Task<StudentDetailsDto?> GetByIdAsync(int id);
        Task<StudentDto> CreateAsync(StudentCreateDto model);
        Task<bool> UpdateAsync(int id, StudentUpdateDto model);
        Task<bool> DeleteAsync(int id);

        // Student self-service
        Task<StudentDetailsDto?> GetMyProfileAsync(int studentId);
        Task<bool> UpdateMyAddressAsync(int studentId, AddressCreateDto model);
        Task<IEnumerable<CourseDto>> GetMyCoursesAsync(int studentId);
        Task<IEnumerable<SubjectDto>> GetMySubjectsAsync(int studentId);
        Task<IEnumerable<TeacherDto>> GetMyTeachersAsync(int studentId);
        Task<bool> EnrollInCourseAsync(int studentId, int courseId);
    }
}
