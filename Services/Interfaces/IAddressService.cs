using TeacherManagement.DTOs;

namespace TeacherManagement.Services.Interfaces
{
    public interface IAddressService
    {
        Task<IEnumerable<AddressDto>> GetAllAsync();
        Task<AddressDto?> GetByIdAsync(int id);
        Task<AddressDto> CreateAsync(AddressCreateDto model);
        Task<bool> UpdateAsync(int id, AddressCreateDto model);
        Task<bool> DeleteAsync(int id);
    }
}
