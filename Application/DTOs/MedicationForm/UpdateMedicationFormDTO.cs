using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.MedicationForm
{
    public class UpdateMedicationFormDTO
    {
        [Required(ErrorMessage = "Medication Form ID is required for update.")]
        [Display(Name = "Medication Form ID")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Name cannot be empty")]
        [StringLength(50, ErrorMessage = "Medication Form Name cannot exceed 50 characters.")]
        [Display(Name = "Name")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Unit cannot be empty")]
        [StringLength(50, ErrorMessage = "Medication Form Unit cannot exceed 50 characters.")]
        [Display(Name = "Unit")]
        public string Unit { get; set; } = null!;
    }
}
