using TeacherManagement.DTOs;

namespace TeacherManagement.Services.Interfaces
{
    public interface ICourseService
    {
        Task<IEnumerable<CourseDto>> GetAllAsync();
        Task<CourseDto?> GetByIdAsync(int id);
        Task<CourseDto> CreateAsync(CourseCreateDto model);
        Task<bool> UpdateAsync(int id, CourseUpdateDto model);
        Task<bool> DeleteAsync(int id);
    }

}
