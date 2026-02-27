using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TeacherManagement.Data;
using TeacherManagement.Models;

namespace TeacherManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class SubjectsController : ControllerBase
    {
        private readonly TeacherContext _db;

        public SubjectsController(TeacherContext db)
        {
            _db = db;
        }

        private static SubjectDto ToDto(Subject s)
        {
            return new SubjectDto
            {
                SubjectId = s.SubjectId,
                Name = s.Name,
                CourseIds = s.Courses?.Select(c => c.CourseId).ToList() ?? new List<int>(),
                CourseNames = s.Courses?.Select(c => c.CourseName).ToList() ?? new List<string>()
            };
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SubjectDto>>> GetAll()
        {
            var list = await _db.Subjects
                .AsNoTracking()
                .Include(s => s.Courses)
                .ToListAsync();

            return Ok(list.Select(ToDto));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SubjectDto>> Get(int id)
        {
            var s = await _db.Subjects
                .AsNoTracking()
                .Include(x => x.Courses)
                .FirstOrDefaultAsync(x => x.SubjectId == id);

            if (s == null) return NotFound();
            return Ok(ToDto(s));
        }

        [HttpPost]
        public async Task<ActionResult> Create(SubjectCreateDto model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var existing = await _db.Subjects.FirstOrDefaultAsync(x => x.Name.ToLower() == model.Name.ToLower());
            if (existing != null) return Conflict("Subject with the same name already exists");

            var newSubject = new Subject { Name = model.Name.Trim() };

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
                newSubject.Courses.Add(course);
            }

            _db.Subjects.Add(newSubject);
            await _db.SaveChangesAsync();

            var created = await _db.Subjects
                .Include(s => s.Courses)
                .FirstOrDefaultAsync(s => s.SubjectId == newSubject.SubjectId);

            return CreatedAtAction(nameof(Get), new { id = newSubject.SubjectId }, ToDto(created!));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, SubjectUpdateDto model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var existing = await _db.Subjects
                .Include(s => s.Courses)
                .FirstOrDefaultAsync(x => x.SubjectId == id);

            if (existing == null) return NotFound();

            existing.Name = model.Name.Trim();

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
            var existing = await _db.Subjects.FindAsync(id);
            if (existing == null) return NotFound();

            _db.Subjects.Remove(existing);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}
