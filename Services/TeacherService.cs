using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TeacherManagement.Data;
using TeacherManagement.DTOs;
using TeacherManagement.Models;

namespace TeacherManagement.Services
{
    public class TeacherService : ITeacherService
    {
        private readonly TeacherContext _db;
        private readonly IMapper _mapper;

        public TeacherService(TeacherContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<PaginationResponse<TeacherDetailsDto>> GetAllAsync(PaginationParams parameters)
        {
            var all = await _db.TeacherDetails.FromSqlRaw("EXEC sp_GetAllTeachers").AsNoTracking().ToListAsync();
            var q = all.AsQueryable();

            if (!string.IsNullOrWhiteSpace(parameters.SearchTerm))
            {
                var s = parameters.SearchTerm.ToLower();
                q = q.Where(t => (t.TeacherName != null && t.TeacherName.ToLower().Contains(s)) ||
                                 (t.BranchName != null && t.BranchName.ToLower().Contains(s)) ||
                                 (t.SubjectNamesCsv != null && t.SubjectNamesCsv.ToLower().Contains(s)));
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
            if (branch == null)
            {
                branch = new Branch { Name = branchName };
                _db.Branches.Add(branch);
                await _db.SaveChangesAsync();
            }

            Address? address = null;
            if (model.Address != null)
            {
                address = _mapper.Map<Address>(model.Address);
                _db.Addresses.Add(address);
                await _db.SaveChangesAsync();
            }

            var subjects = new List<Subject>();
            foreach (var sname in model.SubjectNames ?? Enumerable.Empty<string>())
            {
                var name = sname?.Trim();
                if (string.IsNullOrEmpty(name)) continue;
                var sub = await _db.Subjects.FirstOrDefaultAsync(s => s.Name.ToLower() == name.ToLower());
                if (sub == null)
                {
                    sub = new Subject { Name = name };
                    _db.Subjects.Add(sub);
                    await _db.SaveChangesAsync();
                }
                subjects.Add(sub);
            }

            var teacher = _mapper.Map<Teacher>(model);
            teacher.Branch = branch;
            teacher.Address = address;
            teacher.Subjects = subjects;

            _db.Teachers.Add(teacher);
            await _db.SaveChangesAsync();

            var created = await _db.Teachers
                .Include(t => t.Branch)
                .Include(t => t.Address)
                .Include(t => t.Subjects)
                .FirstOrDefaultAsync(t => t.TeacherId == teacher.TeacherId);

            return _mapper.Map<TeacherDto>(created);
        }

        public async Task<bool> UpdateAsync(int id, TeacherUpdateDto model)
        {
            var existing = await _db.Teachers
                .Include(t => t.Subjects)
                .Include(t => t.Address)
                .Include(t => t.Branch)
                .FirstOrDefaultAsync(t => t.TeacherId == id);

            if (existing == null) return false;

            _mapper.Map(model, existing);

            var branchName = model.BranchName?.Trim();
            if (!string.Equals(existing.Branch?.Name, branchName, StringComparison.OrdinalIgnoreCase))
            {
                var branch = await _db.Branches.FirstOrDefaultAsync(b => b.Name.ToLower() == branchName.ToLower());
                if (branch == null)
                {
                    branch = new Branch { Name = branchName };
                    _db.Branches.Add(branch);
                    await _db.SaveChangesAsync();
                }
                existing.Branch = branch;
                existing.BranchId = branch.BranchId;
            }

            if (model.Address != null)
            {
                if (existing.Address == null)
                {
                    existing.Address = _mapper.Map<Address>(model.Address);
                    _db.Addresses.Add(existing.Address);
                }
                else
                {
                    _mapper.Map(model.Address, existing.Address);
                }
            }
            else
            {
                existing.Address = null;
                existing.AddressId = null;
            }

            existing.Subjects.Clear();
            foreach (var sname in model.SubjectNames ?? Enumerable.Empty<string>())
            {
                var name = sname?.Trim();
                if (string.IsNullOrEmpty(name)) continue;
                var sub = await _db.Subjects.FirstOrDefaultAsync(s => s.Name.ToLower() == name.ToLower());
                if (sub == null)
                {
                    sub = new Subject { Name = name };
                    _db.Subjects.Add(sub);
                    await _db.SaveChangesAsync();
                }
                existing.Subjects.Add(sub);
            }

            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _db.Teachers.FindAsync(id);
            if (existing == null) return false;
            _db.Teachers.Remove(existing);
            await _db.SaveChangesAsync();
            return true;
        }

        // ?? Teacher self-service ??????????????????????????????????????????????

        public async Task<TeacherDetailsDto?> GetMyProfileAsync(int teacherId)
            => await GetByIdAsync(teacherId);

        public async Task<bool> UpdateMyAddressAsync(int teacherId, AddressCreateDto model)
        {
            var teacher = await _db.Teachers.Include(t => t.Address).FirstOrDefaultAsync(t => t.TeacherId == teacherId);
            if (teacher == null) return false;

            if (teacher.Address == null)
            {
                var address = _mapper.Map<Address>(model);
                _db.Addresses.Add(address);
                await _db.SaveChangesAsync();
                teacher.Address = address;
                teacher.AddressId = address.AddressId;
            }
            else
            {
                _mapper.Map(model, teacher.Address);
            }

            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<SubjectDto>> GetMySubjectsAsync(int teacherId)
        {
            var teacher = await _db.Teachers
                .Include(t => t.Subjects).ThenInclude(s => s.Courses)
                .FirstOrDefaultAsync(t => t.TeacherId == teacherId);

            if (teacher == null) return Enumerable.Empty<SubjectDto>();
            return _mapper.Map<IEnumerable<SubjectDto>>(teacher.Subjects);
        }

        public async Task<IEnumerable<StudentDetailsDto>> GetStudentsInMySubjectsAsync(int teacherId)
        {
            var teacher = await _db.Teachers
                .Include(t => t.Subjects).ThenInclude(s => s.Courses).ThenInclude(c => c.Students)
                .FirstOrDefaultAsync(t => t.TeacherId == teacherId);

            if (teacher == null) return Enumerable.Empty<StudentDetailsDto>();

            var studentIds = teacher.Subjects
                .SelectMany(s => s.Courses)
                .SelectMany(c => c.Students)
                .Select(s => s.StudentId)
                .Distinct()
                .ToList();

            var result = new List<StudentDetailsDto>();
            foreach (var sid in studentIds)
            {
                var list = await _db.StudentDetails.FromSqlInterpolated($"EXEC sp_GetStudentById {sid}").ToListAsync();
                var dto = list.FirstOrDefault();
                if (dto != null) result.Add(dto);
            }
            return result;
        }
    }
}
