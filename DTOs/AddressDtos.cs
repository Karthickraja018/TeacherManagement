using System.ComponentModel.DataAnnotations;

namespace TeacherManagement.DTOs
{
    public class AddressDto
    {
        public int AddressId { get; set; }

        [Required]
        [MaxLength(200)]
        public string Line1 { get; set; }

        [MaxLength(200)]
        public string? Line2 { get; set; }

        [MaxLength(100)]
        public string? City { get; set; }

        [MaxLength(100)]
        public string? State { get; set; }

        [MaxLength(20)]
        public string? Zip { get; set; }
    }

    public class AddressCreateDto
    {
        [Required]
        [MaxLength(200)]
        public string Line1 { get; set; }

        [MaxLength(200)]
        public string? Line2 { get; set; }

        [MaxLength(100)]
        public string? City { get; set; }

        [MaxLength(100)]
        public string? State { get; set; }

        [MaxLength(20)]
        public string? Zip { get; set; }
    }
}
