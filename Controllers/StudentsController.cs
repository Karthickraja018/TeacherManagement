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
    public class StudentsController : ControllerBase
    {
        private readonly IStudentService _service;

        public StudentsController(IStudentService service)
        {
            _service = service;
        }

        // ?? Admin / Teacher : browse all students ????????????????????????????

        [HttpGet]
        [Authorize(Roles = "Admin,Teacher")]
        public async Task<ActionResult<PaginationResponse<StudentDetailsDto>>> GetAll([FromQuery] PaginationParams parameters)
        {
            var result = await _service.GetAllAsync(parameters);
            return Ok(result);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Teacher")]
        public async Task<ActionResult<StudentDetailsDto>> Get(int id)
        {
            var dto = await _service.GetByIdAsync(id);
            if (dto == null) return NotFound();
            return Ok(dto);
        }

        // ?? Admin : manage students ??????????????????????????????????????????

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Create(StudentCreateDto model)
        {
            var created = await _service.CreateAsync(model);
            return CreatedAtAction(nameof(Get), new { id = created.StudentId }, created);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Update(int id, StudentUpdateDto model)
        {
            var ok = await _service.UpdateAsync(id, model);
            if (!ok) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Delete(int id)
        {
            var ok = await _service.DeleteAsync(id);
            if (!ok) return NotFound();
            return NoContent();
        }

        // ?? Student self-service ?????????????????????????????????????????????

        /// <summary>View own profile (Student only)</summary>
        [HttpGet]
        [Authorize(Roles = "Student")]
        public async Task<ActionResult<StudentDetailsDto>> MyProfile()
        {
            var studentId = GetLinkedEntityId();
            if (studentId == null) return Forbid();

            var dto = await _service.GetMyProfileAsync(studentId.Value);
            if (dto == null) return NotFound();
            return Ok(dto);
        }

        /// <summary>Update own address (Student only)</summary>
        [HttpPut]
        [Authorize(Roles = "Student")]
        public async Task<ActionResult> UpdateMyAddress(AddressCreateDto model)
        {
            var studentId = GetLinkedEntityId();
            if (studentId == null) return Forbid();

            var ok = await _service.UpdateMyAddressAsync(studentId.Value, model);
            if (!ok) return NotFound();
            return NoContent();
        }

        /// <summary>View own enrolled courses (Student only)</summary>
        [HttpGet]
        [Authorize(Roles = "Student")]
        public async Task<ActionResult<IEnumerable<CourseDto>>> MyCourses()
        {
            var studentId = GetLinkedEntityId();
            if (studentId == null) return Forbid();

            var list = await _service.GetMyCoursesAsync(studentId.Value);
            return Ok(list);
        }

        /// <summary>View subjects in own enrolled courses (Student only)</summary>
        [HttpGet]
        [Authorize(Roles = "Student")]
        public async Task<ActionResult<IEnumerable<SubjectDto>>> MySubjects()
        {
            var studentId = GetLinkedEntityId();
            if (studentId == null) return Forbid();

            var list = await _service.GetMySubjectsAsync(studentId.Value);
            return Ok(list);
        }

        /// <summary>View teachers for subjects in own courses (Student only)</summary>
        [HttpGet]
        [Authorize(Roles = "Student")]
        public async Task<ActionResult<IEnumerable<TeacherDto>>> MyTeachers()
        {
            var studentId = GetLinkedEntityId();
            if (studentId == null) return Forbid();

            var list = await _service.GetMyTeachersAsync(studentId.Value);
            return Ok(list);
        }

        /// <summary>Enroll in a course (Student only)</summary>
        [HttpPost("{courseId}")]
        [Authorize(Roles = "Student")]
        public async Task<ActionResult> Enroll(int courseId)
        {
            var studentId = GetLinkedEntityId();
            if (studentId == null) return Forbid();

            var ok = await _service.EnrollInCourseAsync(studentId.Value, courseId);
            if (!ok) return NotFound();
            return NoContent();
        }

        private int? GetLinkedEntityId()
        {
            var claim = User.FindFirstValue("linked_entity_id");
            return int.TryParse(claim, out var id) ? id : null;
        }
    }
}
