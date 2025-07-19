using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.InventoryItemDetail
{
    public class UpdateInventoryItemDetailDTO
    {
        [Required(ErrorMessage = "Inventory Item Detail ID is required for update.")]
        [Display(Name = "Item Detail Id")]
        public int Id { get; set; }


        [Required(ErrorMessage = "Quantity is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
        [Display(Name = "Quantity")]
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
