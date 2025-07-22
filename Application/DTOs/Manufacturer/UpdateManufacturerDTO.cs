using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Manufacturer
{
    public class UpdateManufacturerDTO
    {
        [Required]
        public int Id { get; set; }

        [Required(ErrorMessage = "Name cannot be empty")]
        [StringLength(100, ErrorMessage = "Manufacturer Name cannot exceed 100 characters.")]
        public string? Name { get; set; }

        [Phone]
        [RegularExpression(@"^[0-9]{10}$", ErrorMessage = "Phone number must be 10 digits")]
        public string? PhoneNumber { get; set; }
    }
}
