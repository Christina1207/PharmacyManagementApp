using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Inventory
{
    public class AddStockDTO
    {
        [Required]
        public int MedicationId { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Price { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        [Required]
        public DateOnly ExpirationDate { get; set; }
    }
}
