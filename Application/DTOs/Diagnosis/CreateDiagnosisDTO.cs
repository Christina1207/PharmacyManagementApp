using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Diagnosis
{
    public class CreateDiagnosisDTO
    {
        [Required(ErrorMessage = "Description cannot be empty")]
        [Display(Name = "Description")]
        public string Description { get; set; } = null!;

    }
}
