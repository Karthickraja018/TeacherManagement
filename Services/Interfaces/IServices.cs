using TeacherManagement.DTOs;
using TeacherManagement.Models;

namespace TeacherManagement.Services
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

    public interface ITeacherService
    {
        Task<PaginationResponse<TeacherDetailsDto>> GetAllAsync(PaginationParams parameters);
        Task<TeacherDetailsDto?> GetByIdAsync(int id);
        Task<TeacherDto> CreateAsync(TeacherCreateDto model);
        Task<bool> UpdateAsync(int id, TeacherUpdateDto model);
        Task<bool> DeleteAsync(int id);

        // Teacher self-service
        Task<TeacherDetailsDto?> GetMyProfileAsync(int teacherId);
        Task<bool> UpdateMyAddressAsync(int teacherId, AddressCreateDto model);
        Task<IEnumerable<SubjectDto>> GetMySubjectsAsync(int teacherId);
        Task<IEnumerable<StudentDetailsDto>> GetStudentsInMySubjectsAsync(int teacherId);
    }

    public interface ICourseService
    {
        Task<IEnumerable<CourseDto>> GetAllAsync();
        Task<CourseDto?> GetByIdAsync(int id);
        Task<CourseDto> CreateAsync(CourseCreateDto model);
        Task<bool> UpdateAsync(int id, CourseUpdateDto model);
        Task<bool> DeleteAsync(int id);
    }

    public interface ISubjectService
    {
        Task<IEnumerable<SubjectDto>> GetAllAsync();
        Task<SubjectDto?> GetByIdAsync(int id);
        Task<SubjectDto> CreateAsync(SubjectCreateDto model);
        Task<bool> UpdateAsync(int id, SubjectUpdateDto model);
        Task<bool> DeleteAsync(int id);
    }

    public interface IAddressService
    {
        Task<IEnumerable<AddressDto>> GetAllAsync();
        Task<AddressDto?> GetByIdAsync(int id);
        Task<AddressDto> CreateAsync(AddressCreateDto model);
        Task<bool> UpdateAsync(int id, AddressCreateDto model);
        Task<bool> DeleteAsync(int id);
    }

    public interface IBranchService
    {
        Task<IEnumerable<BranchDto>> GetAllAsync();
        Task<BranchDto?> GetByIdAsync(int id);
        Task<BranchDto> CreateAsync(BranchDto model);
        Task<bool> UpdateAsync(int id, BranchDto model);
        Task<bool> DeleteAsync(int id);
    }
}
