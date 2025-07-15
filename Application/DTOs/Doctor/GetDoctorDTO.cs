using System.ComponentModel.DataAnnotations;


namespace Application.DTOs.Doctor
{
    public class GetDoctorDTO
    {
        [Display(Name = "Doctor Id")]
        public int Id { get; set; }

        [Display(Name = "First Name")]
        public string FirstName { get; set; } = null!;

        [Display(Name = "Last Name")]
        public string LastName { get; set; } = null!;

        [Display(Name = "Phone Number")]
        public string? PhoneNumber { get; set; }

        [Display(Name = "Speciality")]
        public string Speciality { get; set; } = null!;

    }
}
