using Application.DTOs.MedicationActiveIngredient;
using System.ComponentModel.DataAnnotations;


namespace Application.DTOs.Medication
{
    public class CreateMedicationDTO
    {
        [Required, StringLength(100)]
        public string? Name { get; set; }

        [Required, StringLength(50)]
        public string? Barcode { get; set; }

        [Required, StringLength(50)]
        public string? Dose { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int MinQuantity { get; set; }

        [Required]
        public int ManufacturerId { get; set; }

        [Required]
        public int FormId { get; set; }

        [Required]
        public int ClassId { get; set; }

        [Required]
        [MinLength(1, ErrorMessage = "At least one active ingredient is required.")]
        public List<MedicationActiveIngredientDTO>? ActiveIngredients { get; set; }
    }
}
