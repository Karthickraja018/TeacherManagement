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
    }
}
