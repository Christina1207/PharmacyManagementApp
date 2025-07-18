using System.ComponentModel.DataAnnotations;


namespace Application.DTOs.Department
{
    public class CreateDepartmentDTO
    {
        [Required(ErrorMessage = "Name cannot be empty")]
        [StringLength(50, ErrorMessage = "Department Name cannot exceed 50 characters.")]
        [Display(Name = "Name")]
        public string Name { get; set; } = null!;

    }
}
