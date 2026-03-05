using System.ComponentModel.DataAnnotations;

namespace TeacherManagement.DTOs
{
    public class BranchDto
    {
        public int BranchId { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; }
    }
}
