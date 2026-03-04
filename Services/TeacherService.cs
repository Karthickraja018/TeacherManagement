using Microsoft.EntityFrameworkCore;
using TeacherManagement.Data;
using TeacherManagement.DTOs;
using TeacherManagement.Models;

namespace TeacherManagement.Services
{
    public interface ITeacherService
    {
        Task<PaginationResponse<TeacherDetailsDto>> GetAllAsync(PaginationParams parameters);
        Task<TeacherDetailsDto?> GetByIdAsync(int id);
        Task<TeacherDto> CreateAsync(TeacherCreateDto model);
        Task<bool> UpdateAsync(int id, TeacherUpdateDto model);
        Task<bool> DeleteAsync(int id);
    }

    public class TeacherService : ITeacherService
    {
        private readonly TeacherContext _db;
        public TeacherService(TeacherContext db) { _db = db; }

        public async Task<PaginationResponse<TeacherDetailsDto>> GetAllAsync(PaginationParams parameters)
        {
            var all = await _db.TeacherDetails.FromSqlRaw("EXEC sp_GetAllTeachers").AsNoTracking().ToListAsync();
            var q = all.AsQueryable();
            if (!string.IsNullOrWhiteSpace(parameters.SearchTerm))
            {
                var s = parameters.SearchTerm.ToLower();
                q = q.Where(t => (t.TeacherName != null && t.TeacherName.ToLower().Contains(s)) || (t.BranchName != null && t.BranchName.ToLower().Contains(s)) || (t.SubjectNamesCsv != null && t.SubjectNamesCsv.ToLower().Contains(s)));
            }
            if (!string.IsNullOrWhiteSpace(parameters.SortBy))
            {
                q = parameters.SortBy.ToLower() switch
                {
                    "name" => parameters.SortDescending ? q.OrderByDescending(t => t.TeacherName) : q.OrderBy(t => t.TeacherName),
                    "branch" => parameters.SortDescending ? q.OrderByDescending(t => t.BranchName) : q.OrderBy(t => t.BranchName),
                    "teacherid" => parameters.SortDescending ? q.OrderByDescending(t => t.TeacherId) : q.OrderBy(t => t.TeacherId),
                    _ => q.OrderBy(t => t.TeacherId)
                };
            }
            else q = q.OrderBy(t => t.TeacherId);

            var total = q.Count();
            var paged = q.Skip((parameters.PageNumber - 1) * parameters.PageSize).Take(parameters.PageSize).ToList();
            return new PaginationResponse<TeacherDetailsDto>(paged, total, parameters.PageNumber, parameters.PageSize);
        }

        public async Task<TeacherDetailsDto?> GetByIdAsync(int id)
        {
            var list = await _db.TeacherDetails.FromSqlInterpolated($"EXEC sp_GetTeacherById {id}").ToListAsync();
            return list.FirstOrDefault();
        }

        public async Task<TeacherDto> CreateAsync(TeacherCreateDto model)
        {
            var branchName = model.BranchName?.Trim();
            var branch = await _db.Branches.FirstOrDefaultAsync(b => b.Name.ToLower() == branchName.ToLower());
            if (branch == null) { branch = new Branch { Name = branchName }; _db.Branches.Add(branch); await _db.SaveChangesAsync(); }

            Address? address = null;
            if (model.Address != null) { address = new Address { Line1 = model.Address.Line1, Line2 = model.Address.Line2, City = model.Address.City, State = model.Address.State, Zip = model.Address.Zip }; _db.Addresses.Add(address); await _db.SaveChangesAsync(); }

            var subjects = new List<Subject>();
            foreach (var sname in model.SubjectNames ?? Enumerable.Empty<string>())
            {
                var name = sname?.Trim(); if (string.IsNullOrEmpty(name)) continue;
                var sub = await _db.Subjects.FirstOrDefaultAsync(s => s.Name.ToLower() == name.ToLower());
                if (sub == null) { sub = new Subject { Name = name }; _db.Subjects.Add(sub); await _db.SaveChangesAsync(); }
                subjects.Add(sub);
            }

            var teacher = new Teacher { TeacherName = model.TeacherName, Branch = branch, Address = address, Subjects = subjects };
            _db.Teachers.Add(teacher); await _db.SaveChangesAsync();

            var created = await _db.Teachers.Include(t=>t.Branch).Include(t=>t.Address).Include(t=>t.Subjects).FirstOrDefaultAsync(t => t.TeacherId == teacher.TeacherId);
            return new TeacherDto { TeacherId = created!.TeacherId, TeacherName = created.TeacherName, BranchId = created.BranchId, Branch = new BranchDto { BranchId = created.Branch.BranchId, Name = created.Branch.Name }, AddressId = created.AddressId, Address = created.Address == null ? null : new AddressDto { AddressId = created.Address.AddressId, Line1 = created.Address.Line1, Line2 = created.Address.Line2, City = created.Address.City, State = created.Address.State, Zip = created.Address.Zip }, SubjectIds = created.Subjects.Select(s => s.SubjectId).ToList() };
        }

        public async Task<bool> UpdateAsync(int id, TeacherUpdateDto model)
        {
            var existing = await _db.Teachers.Include(t=>t.Subjects).Include(t=>t.Address).Include(t=>t.Branch).FirstOrDefaultAsync(t=>t.TeacherId == id);
            if (existing == null) return false;
            existing.TeacherName = model.TeacherName;
            var branchName = model.BranchName?.Trim(); if (!string.Equals(existing.Branch?.Name, branchName, StringComparison.OrdinalIgnoreCase)) { var branch = await _db.Branches.FirstOrDefaultAsync(b => b.Name.ToLower() == branchName.ToLower()); if (branch == null) { branch = new Branch { Name = branchName }; _db.Branches.Add(branch); await _db.SaveChangesAsync(); } existing.Branch = branch; existing.BranchId = branch.BranchId; }
            if (model.Address != null) { if (existing.Address == null) { existing.Address = new Address { Line1 = model.Address.Line1, Line2 = model.Address.Line2, City = model.Address.City, State = model.Address.State, Zip = model.Address.Zip }; _db.Addresses.Add(existing.Address); } else { existing.Address.Line1 = model.Address.Line1; existing.Address.Line2 = model.Address.Line2; existing.Address.City = model.Address.City; existing.Address.State = model.Address.State; existing.Address.Zip = model.Address.Zip; } } else { existing.Address = null; existing.AddressId = null; }
            existing.Subjects.Clear(); foreach (var sname in model.SubjectNames ?? Enumerable.Empty<string>()) { var name = sname?.Trim(); if (string.IsNullOrEmpty(name)) continue; var sub = await _db.Subjects.FirstOrDefaultAsync(s => s.Name.ToLower() == name.ToLower()); if (sub == null) { sub = new Subject { Name = name }; _db.Subjects.Add(sub); await _db.SaveChangesAsync(); } existing.Subjects.Add(sub); }
            await _db.SaveChangesAsync(); return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _db.Teachers.FindAsync(id);
            if (existing == null) return false;
            _db.Teachers.Remove(existing); await _db.SaveChangesAsync(); return true;
        }
    }
}
