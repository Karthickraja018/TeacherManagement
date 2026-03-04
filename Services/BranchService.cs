using Microsoft.EntityFrameworkCore;
using TeacherManagement.Data;
using TeacherManagement.DTOs;
using TeacherManagement.Models;

namespace TeacherManagement.Services
{
    public interface IBranchService
    {
        Task<IEnumerable<BranchDto>> GetAllAsync();
        Task<BranchDto?> GetByIdAsync(int id);
        Task<BranchDto> CreateAsync(BranchDto model);
        Task<bool> UpdateAsync(int id, BranchDto model);
        Task<bool> DeleteAsync(int id);
    }

    public class BranchService : IBranchService
    {
        private readonly TeacherContext _db;
        public BranchService(TeacherContext db) { _db = db; }

        public async Task<IEnumerable<BranchDto>> GetAllAsync()
        {
            var list = await _db.Branches.ToListAsync();
            return list.Select(b => new BranchDto { BranchId = b.BranchId, Name = b.Name });
        }

        public async Task<BranchDto?> GetByIdAsync(int id)
        {
            var b = await _db.Branches.FindAsync(id);
            if (b == null) return null;
            return new BranchDto { BranchId = b.BranchId, Name = b.Name };
        }

        public async Task<BranchDto> CreateAsync(BranchDto model)
        {
            var b = new Branch { Name = model.Name?.Trim() };
            _db.Branches.Add(b);
            await _db.SaveChangesAsync();
            return new BranchDto { BranchId = b.BranchId, Name = b.Name };
        }

        public async Task<bool> UpdateAsync(int id, BranchDto model)
        {
            var existing = await _db.Branches.FindAsync(id);
            if (existing == null) return false;
            existing.Name = model.Name;
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _db.Branches.FindAsync(id);
            if (existing == null) return false;
            _db.Branches.Remove(existing);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
