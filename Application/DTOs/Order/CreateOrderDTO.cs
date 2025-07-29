using System.ComponentModel.DataAnnotations;
namespace Application.DTOs.Order
{
    public class CreateOrderDTO
    {
        [Required]
        public int SupplierId { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

        [Required]
        [MinLength(1)]
        public List<CreateOrderItemDTO> OrderItems { get; set; } = new List<CreateOrderItemDTO>();
    }
}
