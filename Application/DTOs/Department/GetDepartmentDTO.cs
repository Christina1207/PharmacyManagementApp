using System.ComponentModel.DataAnnotations;


namespace Application.DTOs.Department
{
    public class GetDepartmentDTO
    {
        [Display(Name = "Department Id")]
        public int Id { get; set; }

        [Display(Name = "Name")]
        public string? Name { get; set; }
    }
}
