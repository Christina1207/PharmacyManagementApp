using System.ComponentModel.DataAnnotations;


namespace Application.DTOs.Doctor
{
    public class CreateDoctorDTO
    {
        [Required(ErrorMessage = "First Name cannot be empty")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = null!;

        [Required(ErrorMessage = "Last Name cannot be empty")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = null!;

        [RegularExpression(@"^[0-9]{10}$", ErrorMessage = "Phone number must be 10 digits")]
        [Display(Name = "Phone Number")]
        public string? PhoneNumber { get; set; }

        [Required(ErrorMessage = "Speciality cannot be empty")]
        [Display(Name = "Speciality")]
        public string Speciality { get; set; } = null!;

    }
}
