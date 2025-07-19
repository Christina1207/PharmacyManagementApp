using System.ComponentModel.DataAnnotations;


namespace Application.DTOs.InventoryItemDetail
{
    public class GetInventoryItemDetailDTO
    {
        [Display(Name = "Item Detail Id")]
        public int Id { get; set; }

        [Display(Name = "Quantity")]
        public int Quantity { get; set; }

        [Display(Name = "Expiration Date")]
        public DateOnly ExpirationDate { get; set; }

        [Display(Name = "Item Id")]
        public int ItemId { get; set; }
    }
}
