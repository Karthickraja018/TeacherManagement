using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using TeacherManagement.Data;
using TeacherManagement.DTOs;
using TeacherManagement.Models;

namespace TeacherManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class StudentsController : ControllerBase
    {
        private readonly IStudentService _service;

        public StudentsController(IStudentService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<PaginationResponse<StudentDetailsDto>>> GetAll([FromQuery] PaginationParams parameters)
        {
            var result = await _service.GetAllAsync(parameters);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<StudentDetailsDto>> Get(int id)
        {
            var dto = await _service.GetByIdAsync(id);
            if (dto == null) return NotFound();
            return Ok(dto);
        }

        [HttpPost]
        public async Task<ActionResult> Create(StudentCreateDto model)
        {
            var created = await _service.CreateAsync(model);
            return CreatedAtAction(nameof(Get), new { id = created.StudentId }, created);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, StudentUpdateDto model)
        {
            var ok = await _service.UpdateAsync(id, model);
            if (!ok) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var ok = await _service.DeleteAsync(id);
            if (!ok) return NotFound();
            return NoContent();
        }
    }

    public interface IStudentService
    {
        Task<PaginationResponse<StudentDetailsDto>> GetAllAsync(PaginationParams parameters);
        Task<StudentDetailsDto?> GetByIdAsync(int id);
        Task<StudentDto> CreateAsync(StudentCreateDto model);
        Task<bool> UpdateAsync(int id, StudentUpdateDto model);
        Task<bool> DeleteAsync(int id);
    }

    public class StudentService : IStudentService
    {
        private readonly TeacherContext _db;
        public StudentService(TeacherContext db)
        {
            _db = db;
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

            // Sorting
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
            // Reuse previous logic in controller for creating student
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
                address = new Address { Line1 = model.Address.Line1, Line2 = model.Address.Line2, City = model.Address.City, State = model.Address.State, Zip = model.Address.Zip };
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

            var student = new Student { Name = model.Name, Branch = branch, Address = address, Courses = courses };
            _db.Students.Add(student);
            await _db.SaveChangesAsync();

            // return full DTO
            var created = await _db.Students.Include(s => s.Branch).Include(s => s.Address).Include(s => s.Courses).FirstOrDefaultAsync(s => s.StudentId == student.StudentId);
            return new StudentDto { StudentId = created!.StudentId, Name = created.Name, BranchId = created.BranchId, Branch = new BranchDto { BranchId = created.Branch.BranchId, Name = created.Branch.Name }, AddressId = created.AddressId, Address = created.Address == null ? null : new AddressDto { AddressId = created.Address.AddressId, Line1 = created.Address.Line1, Line2 = created.Address.Line2, City = created.Address.City, State = created.Address.State, Zip = created.Address.Zip }, CourseIds = created.Courses.Select(c=>c.CourseId).ToList() };
        }

        public async Task<bool> UpdateAsync(int id, StudentUpdateDto model)
        {
            var existing = await _db.Students.Include(s => s.Courses).Include(s => s.Address).Include(s => s.Branch).FirstOrDefaultAsync(x => x.StudentId == id);
            if (existing == null) return false;

            existing.Name = model.Name;
            var branchName = model.BranchName?.Trim();
            if (!string.Equals(existing.Branch?.Name, branchName, StringComparison.OrdinalIgnoreCase))
            {
                var branch = await _db.Branches.FirstOrDefaultAsync(b => b.Name.ToLower() == branchName.ToLower());
                if (branch == null) { branch = new Branch { Name = branchName }; _db.Branches.Add(branch); await _db.SaveChangesAsync(); }
                existing.Branch = branch; existing.BranchId = branch.BranchId;
            }

            if (model.Address != null)
            {
                if (existing.Address == null) { existing.Address = new Address { Line1 = model.Address.Line1, Line2 = model.Address.Line2, City = model.Address.City, State = model.Address.State, Zip = model.Address.Zip }; _db.Addresses.Add(existing.Address); }
                else { existing.Address.Line1 = model.Address.Line1; existing.Address.Line2 = model.Address.Line2; existing.Address.City = model.Address.City; existing.Address.State = model.Address.State; existing.Address.Zip = model.Address.Zip; }
            }
            else { existing.Address = null; existing.AddressId = null; }

            existing.Courses.Clear();
            foreach (var cname in model.CourseNames ?? Enumerable.Empty<string>())
            {
                var name = cname?.Trim(); if (string.IsNullOrEmpty(name)) continue;
                var course = await _db.Courses.FirstOrDefaultAsync(c => c.CourseName.ToLower() == name.ToLower());
                if (course == null) { course = new Course { CourseName = name }; _db.Courses.Add(course); await _db.SaveChangesAsync(); }
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
    }
}
