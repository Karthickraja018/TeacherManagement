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
    }
}
