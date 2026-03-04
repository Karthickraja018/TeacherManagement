using TeacherManagement.DTOs;

namespace TeacherManagement.Services.Interfaces
{
    public interface IBranchService
    {
        Task<IEnumerable<BranchDto>> GetAllAsync();
        Task<BranchDto?> GetByIdAsync(int id);
        Task<BranchDto> CreateAsync(BranchDto model);
        Task<bool> UpdateAsync(int id, BranchDto model);
        Task<bool> DeleteAsync(int id);
    }
}
