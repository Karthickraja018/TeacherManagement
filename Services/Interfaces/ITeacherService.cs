using TeacherManagement.DTOs;

namespace TeacherManagement.Services.Interfaces
{
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
}
