using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TeacherManagement.Data;
using TeacherManagement.DTOs;
using TeacherManagement.Models;
using TeacherManagement.Services.Interfaces;

namespace TeacherManagement.Services
{
    public class StudentService : IStudentService
    {
        private readonly TeacherContext _db;
        private readonly IMapper _mapper;

        public StudentService(TeacherContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<PaginationResponse<StudentDetailsDto>> GetAllAsync(PaginationParams parameters)
        {
            var allResults = await _db.StudentDetails.FromSqlRaw("EXEC sp_GetAllStudents").AsNoTracking().ToListAsync();
            var filteredResults = allResults.AsQueryable();

            if (!string.IsNullOrWhiteSpace(parameters.SearchTerm))
            {
                var searchLower = parameters.SearchTerm.ToLower();
                filteredResults = filteredResults.Where(s =>
                    (s.StudentName != null && s.StudentName.ToLower().Contains(searchLower)) ||
                    (s.BranchName != null && s.BranchName.ToLower().Contains(searchLower)) ||
                    (s.CourseNamesCsv != null && s.CourseNamesCsv.ToLower().Contains(searchLower))
                );
            }

            if (!string.IsNullOrWhiteSpace(parameters.SortBy))
            {
                filteredResults = parameters.SortBy.ToLower() switch
                {
                    "name" => parameters.SortDescending ? filteredResults.OrderByDescending(s => s.StudentName) : filteredResults.OrderBy(s => s.StudentName),
                    "branch" => parameters.SortDescending ? filteredResults.OrderByDescending(s => s.BranchName) : filteredResults.OrderBy(s => s.BranchName),
                    "studentid" => parameters.SortDescending ? filteredResults.OrderByDescending(s => s.StudentId) : filteredResults.OrderBy(s => s.StudentId),
                    _ => filteredResults.OrderBy(s => s.StudentId)
                };
            }
            else filteredResults = filteredResults.OrderBy(s => s.StudentId);

            var totalCount = filteredResults.Count();
            var paged = filteredResults.Skip((parameters.PageNumber - 1) * parameters.PageSize).Take(parameters.PageSize).ToList();

            return new PaginationResponse<StudentDetailsDto>(paged, totalCount, parameters.PageNumber, parameters.PageSize);
        }

        public async Task<StudentDetailsDto?> GetByIdAsync(int id)
        {
            var list = await _db.StudentDetails.FromSqlInterpolated($"EXEC sp_GetStudentById {id}").ToListAsync();
            return list.FirstOrDefault();
        }

        public async Task<StudentDto> CreateAsync(StudentCreateDto model)
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

            var courses = new List<Course>();
            foreach (var cname in model.CourseNames ?? Enumerable.Empty<string>())
            {
                var name = cname?.Trim();
                if (string.IsNullOrEmpty(name)) continue;
                var c = await _db.Courses.FirstOrDefaultAsync(x => x.CourseName.ToLower() == name.ToLower());
                if (c == null)
                {
                    c = new Course { CourseName = name };
                    _db.Courses.Add(c);
                    await _db.SaveChangesAsync();
                }
                courses.Add(c);
            }

            var student = _mapper.Map<Student>(model);
            student.Branch = branch;
            student.Address = address;
            student.Courses = courses;

            _db.Students.Add(student);
            await _db.SaveChangesAsync();

            var created = await _db.Students
                .Include(s => s.Branch)
                .Include(s => s.Address)
                .Include(s => s.Courses)
                .FirstOrDefaultAsync(s => s.StudentId == student.StudentId);

            return _mapper.Map<StudentDto>(created);
        }

        public async Task<bool> UpdateAsync(int id, StudentUpdateDto model)
        {
            var existing = await _db.Students
                .Include(s => s.Courses)
                .Include(s => s.Address)
                .Include(s => s.Branch)
                .FirstOrDefaultAsync(x => x.StudentId == id);

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

            existing.Courses.Clear();
            foreach (var cname in model.CourseNames ?? Enumerable.Empty<string>())
            {
                var name = cname?.Trim();
                if (string.IsNullOrEmpty(name)) continue;
                var course = await _db.Courses.FirstOrDefaultAsync(c => c.CourseName.ToLower() == name.ToLower());
                if (course == null)
                {
                    course = new Course { CourseName = name };
                    _db.Courses.Add(course);
                    await _db.SaveChangesAsync();
                }
                existing.Courses.Add(course);
            }

            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _db.Students.FindAsync(id);
            if (existing == null) return false;
            _db.Students.Remove(existing);
            await _db.SaveChangesAsync();
            return true;
        }

        // ?? Student self-service ??????????????????????????????????????????????

        public async Task<StudentDetailsDto?> GetMyProfileAsync(int studentId)
            => await GetByIdAsync(studentId);

        public async Task<bool> UpdateMyAddressAsync(int studentId, AddressCreateDto model)
        {
            var student = await _db.Students.Include(s => s.Address).FirstOrDefaultAsync(s => s.StudentId == studentId);
            if (student == null) return false;

            if (student.Address == null)
            {
                var address = _mapper.Map<Address>(model);
                _db.Addresses.Add(address);
                await _db.SaveChangesAsync();
                student.Address = address;
                student.AddressId = address.AddressId;
            }
            else
            {
                _mapper.Map(model, student.Address);
            }

            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<CourseDto>> GetMyCoursesAsync(int studentId)
        {
            var student = await _db.Students
                .Include(s => s.Courses).ThenInclude(c => c.Subjects)
                .FirstOrDefaultAsync(s => s.StudentId == studentId);

            if (student == null) return Enumerable.Empty<CourseDto>();
            return _mapper.Map<IEnumerable<CourseDto>>(student.Courses);
        }

        public async Task<IEnumerable<SubjectDto>> GetMySubjectsAsync(int studentId)
        {
            var student = await _db.Students
                .Include(s => s.Courses).ThenInclude(c => c.Subjects).ThenInclude(sub => sub.Courses)
                .FirstOrDefaultAsync(s => s.StudentId == studentId);

            if (student == null) return Enumerable.Empty<SubjectDto>();

            var subjects = student.Courses.SelectMany(c => c.Subjects).DistinctBy(s => s.SubjectId).ToList();
            return _mapper.Map<IEnumerable<SubjectDto>>(subjects);
        }

        public async Task<IEnumerable<TeacherDto>> GetMyTeachersAsync(int studentId)
        {
            var student = await _db.Students
                .Include(s => s.Courses).ThenInclude(c => c.Subjects).ThenInclude(sub => sub.Teachers).ThenInclude(t => t.Branch)
                .FirstOrDefaultAsync(s => s.StudentId == studentId);

            if (student == null) return Enumerable.Empty<TeacherDto>();

            var teachers = student.Courses
                .SelectMany(c => c.Subjects)
                .SelectMany(s => s.Teachers)
                .DistinctBy(t => t.TeacherId)
                .ToList();

            return _mapper.Map<IEnumerable<TeacherDto>>(teachers);
        }

        public async Task<bool> EnrollInCourseAsync(int studentId, int courseId)
        {
            var student = await _db.Students.Include(s => s.Courses).FirstOrDefaultAsync(s => s.StudentId == studentId);
            if (student == null) return false;

            var course = await _db.Courses.FindAsync(courseId);
            if (course == null) return false;

            if (student.Courses.Any(c => c.CourseId == courseId)) return true;

            student.Courses.Add(course);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
