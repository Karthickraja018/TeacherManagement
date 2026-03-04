using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TeacherManagement.Data;
using TeacherManagement.DTOs;
using TeacherManagement.Models;
using TeacherManagement.Services.Interfaces;

namespace TeacherManagement.Services
{
    public class AddressService : IAddressService
    {
        private readonly TeacherContext _db;
        private readonly IMapper _mapper;

        public AddressService(TeacherContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<IEnumerable<AddressDto>> GetAllAsync()
        {
            var list = await _db.Addresses.ToListAsync();
            return _mapper.Map<IEnumerable<AddressDto>>(list);
        }

        public async Task<AddressDto?> GetByIdAsync(int id)
        {
            var a = await _db.Addresses.FindAsync(id);
            if (a == null) return null;
            return _mapper.Map<AddressDto>(a);
        }

        public async Task<AddressDto> CreateAsync(AddressCreateDto model)
        {
            var a = _mapper.Map<Address>(model);
            _db.Addresses.Add(a);
            await _db.SaveChangesAsync();
            return _mapper.Map<AddressDto>(a);
        }

        public async Task<bool> UpdateAsync(int id, AddressCreateDto model)
        {
            var existing = await _db.Addresses.FindAsync(id);
            if (existing == null) return false;
            _mapper.Map(model, existing);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _db.Addresses.FindAsync(id);
            if (existing == null) return false;
            _db.Addresses.Remove(existing);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
