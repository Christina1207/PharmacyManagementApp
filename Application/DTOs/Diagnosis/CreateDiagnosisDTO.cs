using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Diagnosis
{
    public class CreateDiagnosisDTO
    {
        [Required(ErrorMessage = "Description cannot be empty")]
        [StringLength(255, ErrorMessage = "Diagnosis Description cannot exceed 255 characters.")]
        [Display(Name = "Description")]
        public string Description { get; set; } = null!;

    }
}
