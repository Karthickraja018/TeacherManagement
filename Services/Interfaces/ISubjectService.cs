using TeacherManagement.DTOs;

namespace TeacherManagement.Services.Interfaces
{
    public interface ISubjectService
    {
        Task<IEnumerable<SubjectDto>> GetAllAsync();
        Task<SubjectDto?> GetByIdAsync(int id);
        Task<SubjectDto> CreateAsync(SubjectCreateDto model);
        Task<bool> UpdateAsync(int id, SubjectUpdateDto model);
        Task<bool> DeleteAsync(int id);
    }
}
