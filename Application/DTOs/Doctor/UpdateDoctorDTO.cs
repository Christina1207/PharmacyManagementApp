using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Doctor
{
    public class UpdateDoctorDTO
    {

        [Required(ErrorMessage = "Specify the Id of the doctor to edit")]
        [Display(Name = "Doctor Id")]
        public int Id { get; set; }

        [Required(ErrorMessage = "First Name cannot be empty")]
        [StringLength(50, ErrorMessage = "First Name cannot exceed 50 characters.")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = null!;

        [Required(ErrorMessage = "Last Name cannot be empty")]
        [StringLength(50, ErrorMessage = "Last Name cannot exceed 50 characters.")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = null!;

        [RegularExpression(@"^[0-9]{10}$", ErrorMessage = "Phone number must be 10 digits")]
        [Display(Name = "Phone Number")]
        public string? PhoneNumber { get; set; }

        [Required(ErrorMessage = "Speciality cannot be empty")]
        [StringLength(50, ErrorMessage = "Speciality cannot exceed 50 characters.")]
        [Display(Name = "Speciality")]
        public string Speciality { get; set; } = null!;
    }
}
