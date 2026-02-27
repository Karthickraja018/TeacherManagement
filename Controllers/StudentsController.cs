using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TeacherManagement.Data;
using TeacherManagement.Models;

namespace TeacherManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class StudentsController : ControllerBase
    {
        private readonly TeacherContext _db;

        public StudentsController(TeacherContext db)
        {
            _db = db;
        }

        private static StudentDto ToDto(Student s)
        {
            return new StudentDto
            {
                StudentId = s.StudentId,
                Name = s.Name,
                BranchId = s.BranchId,
                Branch = s.Branch == null ? null : new BranchDto { BranchId = s.Branch.BranchId, Name = s.Branch.Name },
                AddressId = s.AddressId,
                Address = s.Address == null ? null : new AddressDto { AddressId = s.Address.AddressId, Line1 = s.Address.Line1, Line2 = s.Address.Line2, City = s.Address.City, State = s.Address.State, Zip = s.Address.Zip },
                CourseIds = s.Courses?.Select(c => c.CourseId).ToList() ?? new List<int>()
            };
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<StudentDto>>> GetAll()
        {
            var list = await _db.Students
                .AsNoTracking()
                .Include(s => s.Branch)
                .Include(s => s.Address)
                .Include(s => s.Courses)
                .ToListAsync();

            return Ok(list.Select(ToDto));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<StudentDto>> Get(int id)
        {
            var s = await _db.Students
                .AsNoTracking()
                .Include(x => x.Branch)
                .Include(x => x.Address)
                .Include(x => x.Courses)
                .FirstOrDefaultAsync(x => x.StudentId == id);

            if (s == null) return NotFound();
            return Ok(ToDto(s));
        }

        [HttpPost]
        public async Task<ActionResult> Create(StudentCreateDto model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            // Find or create branch by name (case-insensitive)
            var branchName = model.BranchName?.Trim();
            var branch = await _db.Branches.FirstOrDefaultAsync(b => b.Name.ToLower() == branchName.ToLower());
            if (branch == null)
            {
                branch = new Branch { Name = branchName };
                _db.Branches.Add(branch);
                await _db.SaveChangesAsync();
            }

            // Handle address if provided
            Address? address = null;
            if (model.Address != null)
            {
                address = new Address
                {
                    Line1 = model.Address.Line1,
                    Line2 = model.Address.Line2,
                    City = model.Address.City,
                    State = model.Address.State,
                    Zip = model.Address.Zip
                };
                _db.Addresses.Add(address);
                await _db.SaveChangesAsync();
            }

            // Find or create courses by name
            var courses = new List<Course>();
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
                courses.Add(course);
            }

            var student = new Student
            {
                Name = model.Name,
                Branch = branch,
                Address = address,
                Courses = courses
            };

            _db.Students.Add(student);
            await _db.SaveChangesAsync();

            var created = await _db.Students
                .Include(s => s.Branch)
                .Include(s => s.Address)
                .Include(s => s.Courses)
                .FirstOrDefaultAsync(s => s.StudentId == student.StudentId);

            return CreatedAtAction(nameof(Get), new { id = student.StudentId }, ToDto(created!));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, StudentUpdateDto model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var existing = await _db.Students
                .Include(s => s.Courses)
                .Include(s => s.Address)
                .Include(s => s.Branch)
                .FirstOrDefaultAsync(x => x.StudentId == id);

            if (existing == null) return NotFound();

            existing.Name = model.Name;

            // Branch
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

            // Address: replace or create
            if (model.Address != null)
            {
                if (existing.Address == null)
                {
                    existing.Address = new Address
                    {
                        Line1 = model.Address.Line1,
                        Line2 = model.Address.Line2,
                        City = model.Address.City,
                        State = model.Address.State,
                        Zip = model.Address.Zip
                    };
                    _db.Addresses.Add(existing.Address);
                }
                else
                {
                    existing.Address.Line1 = model.Address.Line1;
                    existing.Address.Line2 = model.Address.Line2;
                    existing.Address.City = model.Address.City;
                    existing.Address.State = model.Address.State;
                    existing.Address.Zip = model.Address.Zip;
                }
            }
            else
            {
                // If client clears address, remove reference
                existing.Address = null;
                existing.AddressId = null;
            }

            // Courses - clear and set by names
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

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var existing = await _db.Students.FindAsync(id);
            if (existing == null) return NotFound();

            _db.Students.Remove(existing);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}
