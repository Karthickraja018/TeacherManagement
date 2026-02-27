using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TeacherManagement.Data;
using TeacherManagement.Models;

namespace TeacherManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class CoursesController : ControllerBase
    {
        private readonly TeacherContext _db;

        public CoursesController(TeacherContext db)
        {
            _db = db;
        }

        private static CourseDto ToDto(Course c)
        {
            return new CourseDto
            {
                CourseId = c.CourseId,
                CourseName = c.CourseName,
                SubjectIds = c.Subjects?.Select(s => s.SubjectId).ToList() ?? new List<int>(),
                SubjectNames = c.Subjects?.Select(s => s.Name).ToList() ?? new List<string>()
            };
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CourseDto>>> GetAll()
        {
            var list = await _db.Courses
                .AsNoTracking()
                .Include(c => c.Subjects)
                .ToListAsync();

            return Ok(list.Select(ToDto));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CourseDto>> Get(int id)
        {
            var c = await _db.Courses
                .AsNoTracking()
                .Include(x => x.Subjects)
                .FirstOrDefaultAsync(x => x.CourseId == id);

            if (c == null) return NotFound();
            return Ok(ToDto(c));
        }

        [HttpPost]
        public async Task<ActionResult> Create(CourseCreateDto model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var course = await _db.Courses.FirstOrDefaultAsync(x => x.CourseName.ToLower() == model.CourseName.ToLower());
            if (course != null) return Conflict("Course with the same name already exists");

            var newCourse = new Course { CourseName = model.CourseName.Trim() };

            // Attach or create subjects
            foreach (var sname in model.SubjectNames ?? Enumerable.Empty<string>())
            {
                var name = sname?.Trim();
                if (string.IsNullOrEmpty(name)) continue;
                var subject = await _db.Subjects.FirstOrDefaultAsync(s => s.Name.ToLower() == name.ToLower());
                if (subject == null)
                {
                    subject = new Subject { Name = name };
                    _db.Subjects.Add(subject);
                    await _db.SaveChangesAsync();
                }
                newCourse.Subjects.Add(subject);
            }

            _db.Courses.Add(newCourse);
            await _db.SaveChangesAsync();

            var created = await _db.Courses
                .Include(c => c.Subjects)
                .FirstOrDefaultAsync(c => c.CourseId == newCourse.CourseId);

            return CreatedAtAction(nameof(Get), new { id = newCourse.CourseId }, ToDto(created!));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, CourseUpdateDto model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var existing = await _db.Courses
                .Include(c => c.Subjects)
                .FirstOrDefaultAsync(x => x.CourseId == id);

            if (existing == null) return NotFound();

            existing.CourseName = model.CourseName.Trim();

            existing.Subjects.Clear();
            foreach (var sname in model.SubjectNames ?? Enumerable.Empty<string>())
            {
                var name = sname?.Trim();
                if (string.IsNullOrEmpty(name)) continue;
                var subject = await _db.Subjects.FirstOrDefaultAsync(s => s.Name.ToLower() == name.ToLower());
                if (subject == null)
                {
                    subject = new Subject { Name = name };
                    _db.Subjects.Add(subject);
                    await _db.SaveChangesAsync();
                }
                existing.Subjects.Add(subject);
            }

            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var existing = await _db.Courses.FindAsync(id);
            if (existing == null) return NotFound();

            _db.Courses.Remove(existing);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}
