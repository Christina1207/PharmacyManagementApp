
namespace Application.DTOs.Inventory
{
    public class GetInventoryItemDTO
    {
        public int Id { get; set; }
        public int MedicationId { get; set; }
        public string? MedicationName { get; set; }
        public decimal Price { get; set; }
        public int TotalQuantity { get; set; }
        public List<GetInventoryBatchDTO>? Batches { get; set; }

    }
}
