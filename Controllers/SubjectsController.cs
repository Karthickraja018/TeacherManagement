using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TeacherManagement.DTOs;
using TeacherManagement.Services;

namespace TeacherManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    [Authorize]
    public class SubjectsController : ControllerBase
    {
        private readonly ISubjectService _service;
        public SubjectsController(ISubjectService service) { _service = service; }

        /// <summary>Browse all subjects (Admin, Teacher, Student)</summary>
        [HttpGet]
        [Authorize(Roles = "Admin,Teacher,Student")]
        public async Task<ActionResult<IEnumerable<SubjectDto>>> GetAll()
        {
            var list = await _service.GetAllAsync();
            return Ok(list);
        }

        /// <summary>Get subject by id (Admin, Teacher, Student)</summary>
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Teacher,Student")]
        public async Task<ActionResult<SubjectDto>> Get(int id)
        {
            var s = await _service.GetByIdAsync(id);
            if (s == null) return NotFound();
            return Ok(s);
        }

        /// <summary>Create subject (Admin only)</summary>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Create(SubjectCreateDto model)
        {
            var created = await _service.CreateAsync(model);
            return CreatedAtAction(nameof(Get), new { id = created.SubjectId }, created);
        }

        /// <summary>Update subject (Admin only)</summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Update(int id, SubjectUpdateDto model)
        {
            var ok = await _service.UpdateAsync(id, model);
            if (!ok) return NotFound();
            return NoContent();
        }

        /// <summary>Delete subject (Admin only)</summary>
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
