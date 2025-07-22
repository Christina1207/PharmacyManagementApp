using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Manufacturer
{
    public class CreateManufacturerDTO
    {

        [Required(ErrorMessage = "Name cannot be empty")]
        [StringLength(100, ErrorMessage = "Manufacturer Name cannot exceed 100 characters.")]
        public string? Name { get; set; }

        [RegularExpression(@"^[0-9]{10}$", ErrorMessage = "Phone number must be 10 digits")]
        public string? PhoneNumber { get; set; }
    }
}
