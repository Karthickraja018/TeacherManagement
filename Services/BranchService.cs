using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TeacherManagement.Data;
using TeacherManagement.DTOs;
using TeacherManagement.Models;
using TeacherManagement.Services.Interfaces;

namespace TeacherManagement.Services
{
    public class BranchService : IBranchService
    {
        private readonly TeacherContext _db;
        private readonly IMapper _mapper;

        public BranchService(TeacherContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<IEnumerable<BranchDto>> GetAllAsync()
        {
            var list = await _db.Branches.ToListAsync();
            return _mapper.Map<IEnumerable<BranchDto>>(list);
        }

        public async Task<BranchDto?> GetByIdAsync(int id)
        {
            var b = await _db.Branches.FindAsync(id);
            if (b == null) return null;
            return _mapper.Map<BranchDto>(b);
        }

        public async Task<BranchDto> CreateAsync(BranchDto model)
        {
            var b = _mapper.Map<Branch>(model);
            _db.Branches.Add(b);
            await _db.SaveChangesAsync();
            return _mapper.Map<BranchDto>(b);
        }

        public async Task<bool> UpdateAsync(int id, BranchDto model)
        {
            var existing = await _db.Branches.FindAsync(id);
            if (existing == null) return false;
            _mapper.Map(model, existing);
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
