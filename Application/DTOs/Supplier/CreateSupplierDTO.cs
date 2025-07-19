using Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Supplier
{
    public class CreateSupplierDTO
    {
        [Required(ErrorMessage = "Name cannot be empty")]
        [StringLength(100, ErrorMessage = "Supplier Name cannot exceed 100 characters.")]
        [Display(Name = "Name")]
        public string Name { get; set; } = null!;

        [RegularExpression(@"^[0-9]{10}$", ErrorMessage = "Phone number must be 10 digits")]
        [Display(Name = "Phone Number")]
        public string? PhoneNumber { get; set; }
    }

}
