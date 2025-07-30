using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.InventoryCheck
{
    public class CreateInventoryCheckItemDTO
    {
        [Required]
        public int MedicationId { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Counted quantity cannot be negative.")]
        public int CountedQuantity { get; set; }
    }
}
