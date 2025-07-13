using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Supplier
{
    public class UpdateSupplierDTO
    {
        [Required(ErrorMessage ="Specify the Id of the supplier to edit")]
        [Display (Name = "Supplier Id")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Name cannot be empty")]
        [Display(Name = "Name")]
        public string Name { get; set; } = null!;

        [RegularExpression(@"^[0-9]{10}$", ErrorMessage = "Phone number must be 10 digits")]
        [Display(Name = "Phone Number")]
        public string? PhoneNumber { get; set; }
    }
}
