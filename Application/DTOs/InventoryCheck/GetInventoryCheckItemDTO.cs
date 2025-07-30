namespace Application.DTOs.InventoryCheck
{
    public class GetInventoryCheckItemDTO
    {
        public int MedicationId { get; set; }
        public string? MedicationName { get; set; }
        public int ExpectedQuantity { get; set; }
        public int CountedQuantity { get; set; }
        public int Variance => CountedQuantity - ExpectedQuantity;
    }
}
