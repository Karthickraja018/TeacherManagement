using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TeacherManagement.DTOs;
using TeacherManagement.Services.Interfaces;

namespace TeacherManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    [Authorize]
    public class BranchesController : ControllerBase
    {
        private readonly IBranchService _service;
        public BranchesController(IBranchService service) { _service = service; }

        /// <summary>Browse all branches (Admin, Teacher, Student)</summary>
        [HttpGet]
        [Authorize(Roles = "Admin,Teacher,Student")]
        public async Task<ActionResult<IEnumerable<BranchDto>>> GetAll()
        {
            var list = await _service.GetAllAsync();
            return Ok(list);
        }

        /// <summary>Get branch by id (Admin, Teacher, Student)</summary>
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Teacher,Student")]
        public async Task<ActionResult<BranchDto>> Get(int id)
        {
            var b = await _service.GetByIdAsync(id);
            if (b == null) return NotFound();
            return Ok(b);
        }

        /// <summary>Create branch (Admin only)</summary>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Create(BranchDto model)
        {
            var created = await _service.CreateAsync(model);
            return CreatedAtAction(nameof(Get), new { id = created.BranchId }, created);
        }

        /// <summary>Update branch (Admin only)</summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Update(int id, BranchDto model)
        {
            var ok = await _service.UpdateAsync(id, model);
            if (!ok) return NotFound();
            return NoContent();
        }

        /// <summary>Delete branch (Admin only)</summary>
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
