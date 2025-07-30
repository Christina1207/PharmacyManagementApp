using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.User
{
    public class UpdateUserDTO
    {
        [Required]
        public string Role { get; set; } = string.Empty;

        [Required]
        public bool IsActive { get; set; }
    }
}
