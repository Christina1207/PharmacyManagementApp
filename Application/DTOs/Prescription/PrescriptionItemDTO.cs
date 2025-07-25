using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Prescription
{
    public class PrescriptionItemDTO
    {
        [Required]
        public int MedicationId { get; set;}

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
        public int Quantity {  get; set;}


    }
}
