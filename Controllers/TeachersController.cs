using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TeacherManagement.Data;
using TeacherManagement.Models;

namespace TeacherManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class TeachersController : ControllerBase
    {
        private readonly TeacherContext _db;

        public TeachersController(TeacherContext db)
        {
            _db = db;
        }

        private static TeacherDto ToDto(Teacher t)
        {
            return new TeacherDto
            {
                TeacherId = t.TeacherId,
                TeacherName = t.TeacherName,
                BranchId = t.BranchId,
                Branch = t.Branch == null ? null : new BranchDto { BranchId = t.Branch.BranchId, Name = t.Branch.Name },
                AddressId = t.AddressId,
                Address = t.Address == null ? null : new AddressDto { AddressId = t.Address.AddressId, Line1 = t.Address.Line1, Line2 = t.Address.Line2, City = t.Address.City, State = t.Address.State, Zip = t.Address.Zip },
                SubjectIds = t.Subjects?.Select(s => s.SubjectId).ToList() ?? new List<int>()
            };
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TeacherDto>>> GetAllTeachers()
        {
            var list = await _db.Teachers
                .AsNoTracking()
                .Include(t => t.Branch)
                .Include(t => t.Address)
                .Include(t => t.Subjects)
                .ToListAsync();

            return Ok(list.Select(ToDto));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TeacherDto>> GetTeacherById(int id)
        {
            var t = await _db.Teachers
                .AsNoTracking()
                .Include(x => x.Branch)
                .Include(x => x.Address)
                .Include(x => x.Subjects)
                .FirstOrDefaultAsync(x => x.TeacherId == id);

            if (t == null) return NotFound();
            return Ok(ToDto(t));
        }

        [HttpPost]
        public async Task<ActionResult> PostTeacher(TeacherCreateDto model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            // Branch
            var branchName = model.BranchName?.Trim();
            var branch = await _db.Branches.FirstOrDefaultAsync(b => b.Name.ToLower() == branchName.ToLower());
            if (branch == null)
            {
                branch = new Branch { Name = branchName };
                _db.Branches.Add(branch);
                await _db.SaveChangesAsync();
            }

            // Address
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

            // Subjects
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

            var teacher = new Teacher
            {
                TeacherName = model.TeacherName,
                Branch = branch,
                Address = address,
                Subjects = subjects
            };

            _db.Teachers.Add(teacher);
            await _db.SaveChangesAsync();

            var created = await _db.Teachers
                .Include(t => t.Branch)
                .Include(t => t.Address)
                .Include(t => t.Subjects)
                .FirstOrDefaultAsync(t => t.TeacherId == teacher.TeacherId);

            return CreatedAtAction(nameof(GetTeacherById), new { id = teacher.TeacherId }, ToDto(created!));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateTeacher(int id, TeacherUpdateDto model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var existing = await _db.Teachers
                .Include(t => t.Subjects)
                .Include(t => t.Address)
                .Include(t => t.Branch)
                .FirstOrDefaultAsync(x => x.TeacherId == id);

            if (existing == null) return NotFound();

            existing.TeacherName = model.TeacherName;

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

            // Address
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
                existing.Address = null;
                existing.AddressId = null;
            }

            // Subjects
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

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteTeacher(int id)
        {
            var existing = await _db.Teachers.FindAsync(id);
            if (existing == null) return NotFound();

            _db.Teachers.Remove(existing);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}
