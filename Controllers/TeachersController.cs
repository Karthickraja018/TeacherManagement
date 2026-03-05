using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TeacherManagement.DTOs;
using TeacherManagement.Services;

namespace TeacherManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    [Authorize]
    public class TeachersController : ControllerBase
    {
        private readonly ITeacherService _service;

        public TeachersController(ITeacherService service)
        {
            _service = service;
        }

        // ?? Admin : browse all teachers ??????????????????????????????????????

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<PaginationResponse<TeacherDetailsDto>>> GetAllTeachers([FromQuery] PaginationParams parameters)
        {
            var result = await _service.GetAllAsync(parameters);
            return Ok(result);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<TeacherDetailsDto>> GetTeacherById(int id)
        {
            var dto = await _service.GetByIdAsync(id);
            if (dto == null) return NotFound();
            return Ok(dto);
        }

        // ?? Admin : manage teachers ??????????????????????????????????????????

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> PostTeacher(TeacherCreateDto model)
        {
            var created = await _service.CreateAsync(model);
            return CreatedAtAction(nameof(GetTeacherById), new { id = created.TeacherId }, created);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> UpdateTeacher(int id, TeacherUpdateDto model)
        {
            var ok = await _service.UpdateAsync(id, model);
            if (!ok) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteTeacher(int id)
        {
            var ok = await _service.DeleteAsync(id);
            if (!ok) return NotFound();
            return NoContent();
        }

        // ?? Teacher self-service ?????????????????????????????????????????????

        /// <summary>View own profile (Teacher only)</summary>
        [HttpGet]
        [Authorize(Roles = "Teacher")]
        public async Task<ActionResult<TeacherDetailsDto>> MyProfile()
        {
            var teacherId = GetLinkedEntityId();
            if (teacherId == null) return Forbid();

            var dto = await _service.GetMyProfileAsync(teacherId.Value);
            if (dto == null) return NotFound();
            return Ok(dto);
        }

        /// <summary>Update own address (Teacher only)</summary>
        [HttpPut]
        [Authorize(Roles = "Teacher")]
        public async Task<ActionResult> UpdateMyAddress(AddressCreateDto model)
        {
            var teacherId = GetLinkedEntityId();
            if (teacherId == null) return Forbid();

            var ok = await _service.UpdateMyAddressAsync(teacherId.Value, model);
            if (!ok) return NotFound();
            return NoContent();
        }

        /// <summary>View subjects I teach (Teacher only)</summary>
        [HttpGet]
        [Authorize(Roles = "Teacher")]
        public async Task<ActionResult<IEnumerable<SubjectDto>>> MySubjects()
        {
            var teacherId = GetLinkedEntityId();
            if (teacherId == null) return Forbid();

            var list = await _service.GetMySubjectsAsync(teacherId.Value);
            return Ok(list);
        }

        /// <summary>View students enrolled in my subjects (Teacher only)</summary>
        [HttpGet]
        [Authorize(Roles = "Teacher")]
        public async Task<ActionResult<IEnumerable<StudentDetailsDto>>> MyStudents()
        {
            var teacherId = GetLinkedEntityId();
            if (teacherId == null) return Forbid();

            var list = await _service.GetStudentsInMySubjectsAsync(teacherId.Value);
            return Ok(list);
        }

        private int? GetLinkedEntityId()
        {
            var claim = User.FindFirstValue("linked_entity_id");
            return int.TryParse(claim, out var id) ? id : null;
        }
    }
}
