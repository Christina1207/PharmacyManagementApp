using System.ComponentModel.DataAnnotations;


namespace Application.DTOs.Diagnosis
{
    public class UpdateDiagnosisDTO
    {
        [Required(ErrorMessage = "Specify the Id of the diagnosis to edit")]
        [Display(Name = "Diagnosis Id")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Description cannot be empty")]
        [StringLength(255, ErrorMessage = "Diagnosis Description cannot exceed 255 characters.")]
        [Display(Name = "Description")]
        public string Description { get; set; } = null!;
    }
}
