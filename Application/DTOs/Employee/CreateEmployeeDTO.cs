using System.ComponentModel.DataAnnotations;


namespace Application.DTOs.Employee
{
    public class CreateEmployeeDTO
    {
        [Required]
        public string? FirstName { get; set; }
        [Required]
        public string? LastName { get; set; }
        [Required]
        public DateOnly DateOfBirth { get; set; }

        [Required]
        public int DepartmentId { get; set; }
        public string? PhoneNumber { get; set; }
    }
}
