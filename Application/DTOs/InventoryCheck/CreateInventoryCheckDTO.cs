using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.InventoryCheck
{
    public class CreateInventoryCheckDTO
    {
        [Required]
        [StringLength(255)]
        public string Notes { get; set; } = string.Empty;

        [Required]
        [MinLength(1, ErrorMessage = "An inventory check must contain at least one item.")]
        public List<CreateInventoryCheckItemDTO> Items { get; set; } = new List<CreateInventoryCheckItemDTO>();
    }
}
