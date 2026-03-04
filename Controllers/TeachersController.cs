using Microsoft.AspNetCore.Mvc;
using TeacherManagement.DTOs;
using TeacherManagement.Models;
using TeacherManagement.Services;

namespace TeacherManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class TeachersController : ControllerBase
    {
        private readonly ITeacherService _service;

        public TeachersController(ITeacherService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<PaginationResponse<TeacherDetailsDto>>> GetAllTeachers([FromQuery] PaginationParams parameters)
        {
            var result = await _service.GetAllAsync(parameters);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TeacherDetailsDto>> GetTeacherById(int id)
        {
            var dto = await _service.GetByIdAsync(id);
            if (dto == null) return NotFound();
            return Ok(dto);
        }

        [HttpPost]
        public async Task<ActionResult> PostTeacher(TeacherCreateDto model)
        {
            var created = await _service.CreateAsync(model);
            return CreatedAtAction(nameof(GetTeacherById), new { id = created.TeacherId }, created);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateTeacher(int id, TeacherUpdateDto model)
        {
            var ok = await _service.UpdateAsync(id, model);
            if (!ok) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteTeacher(int id)
        {
            var ok = await _service.DeleteAsync(id);
            if (!ok) return NotFound();
            return NoContent();
        }
    }
}
