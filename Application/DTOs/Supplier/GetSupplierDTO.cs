using System.ComponentModel.DataAnnotations;


namespace Application.DTOs.Supplier
{
    public class GetSupplierDTO
    {
        [Display(Name = "Supplier Id")]
        public int Id { get; set; }

        [Display(Name = "Name")]
        public string Name { get; set; } = null!;

        [Display(Name = "Phone Number")]
        public string? PhoneNumber { get; set; }


    }
}
