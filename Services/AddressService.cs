using Microsoft.EntityFrameworkCore;
using TeacherManagement.Data;
using TeacherManagement.DTOs;
using TeacherManagement.Models;

namespace TeacherManagement.Services
{
    public interface IAddressService
    {
        Task<IEnumerable<AddressDto>> GetAllAsync();
        Task<AddressDto?> GetByIdAsync(int id);
        Task<AddressDto> CreateAsync(AddressCreateDto model);
        Task<bool> UpdateAsync(int id, AddressCreateDto model);
        Task<bool> DeleteAsync(int id);
    }

    public class AddressService : IAddressService
    {
        private readonly TeacherContext _db;
        public AddressService(TeacherContext db) { _db = db; }

        public async Task<IEnumerable<AddressDto>> GetAllAsync()
        {
            var list = await _db.Addresses.ToListAsync();
            return list.Select(a => new AddressDto { AddressId = a.AddressId, Line1 = a.Line1, Line2 = a.Line2, City = a.City, State = a.State, Zip = a.Zip });
        }

        public async Task<AddressDto?> GetByIdAsync(int id)
        {
            var a = await _db.Addresses.FindAsync(id);
            if (a == null) return null;
            return new AddressDto { AddressId = a.AddressId, Line1 = a.Line1, Line2 = a.Line2, City = a.City, State = a.State, Zip = a.Zip };
        }

        public async Task<AddressDto> CreateAsync(AddressCreateDto model)
        {
            var a = new Address { Line1 = model.Line1, Line2 = model.Line2, City = model.City, State = model.State, Zip = model.Zip };
            _db.Addresses.Add(a); await _db.SaveChangesAsync();
            return new AddressDto { AddressId = a.AddressId, Line1 = a.Line1, Line2 = a.Line2, City = a.City, State = a.State, Zip = a.Zip };
        }

        public async Task<bool> UpdateAsync(int id, AddressCreateDto model)
        {
            var existing = await _db.Addresses.FindAsync(id); if (existing == null) return false; existing.Line1 = model.Line1; existing.Line2 = model.Line2; existing.City = model.City; existing.State = model.State; existing.Zip = model.Zip; await _db.SaveChangesAsync(); return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _db.Addresses.FindAsync(id); if (existing == null) return false; _db.Addresses.Remove(existing); await _db.SaveChangesAsync(); return true;
        }
    }
}
