using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TeacherManagement.DTOs;
using TeacherManagement.Services;

namespace TeacherManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    [Authorize]
    public class CoursesController : ControllerBase
    {
        private readonly ICourseService _service;
        public CoursesController(ICourseService service) { _service = service; }

        /// <summary>Browse all courses (Admin, Teacher, Student)</summary>
        [HttpGet]
        [Authorize(Roles = "Admin,Teacher,Student")]
        public async Task<ActionResult<IEnumerable<CourseDto>>> GetAll()
        {
            var list = await _service.GetAllAsync();
            return Ok(list);
        }

        /// <summary>Get course by id (Admin, Teacher, Student)</summary>
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Teacher,Student")]
        public async Task<ActionResult<CourseDto>> Get(int id)
        {
            var c = await _service.GetByIdAsync(id);
            if (c == null) return NotFound();
            return Ok(c);
        }

        /// <summary>Create course (Admin only)</summary>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Create(CourseCreateDto model)
        {
            var created = await _service.CreateAsync(model);
            return CreatedAtAction(nameof(Get), new { id = created.CourseId }, created);
        }

        /// <summary>Update course (Admin only)</summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Update(int id, CourseUpdateDto model)
        {
            var ok = await _service.UpdateAsync(id, model);
            if (!ok) return NotFound();
            return NoContent();
        }

        /// <summary>Delete course (Admin only)</summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Delete(int id)
        {
            var ok = await _service.DeleteAsync(id);
            if (!ok) return NotFound();
            return NoContent();
        }
    }
}