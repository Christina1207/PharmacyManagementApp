using System.ComponentModel.DataAnnotations;


namespace Application.DTOs.Department
{
    public class CreateDepartmentDTO
    {
        [Required(ErrorMessage = "Name cannot be empty")]
        [Display(Name = "Name")]
        public string Name { get; set; } = null!;

    }
}
