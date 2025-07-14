using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Department
{
    public class UpdateDepartmentDTO
    {
        [Required(ErrorMessage = "Specify the Id of the department to edit")]
        [Display(Name = "Department Id")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Name cannot be empty")]
        [Display(Name = "Name")]
        public string Name { get; set; } = null!;
    }
}
