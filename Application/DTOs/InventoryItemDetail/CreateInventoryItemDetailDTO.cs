using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.InventoryItemDetail
{
    public class CreateInventoryItemDetailDTO
    {
        [Required(ErrorMessage = "Quantity is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
        [Display (Name = "Quantity")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "Expiration Date is required.")]
        [Display(Name = "Expiration Date")]
        public DateOnly ExpirationDate { get; set; }

        [Required(ErrorMessage = "Item ID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Item ID must be a positive integer.")]
        [Display(Name = "Item Id")]
        public int ItemId { get; set; }
    }
}
