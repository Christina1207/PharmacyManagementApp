namespace Application.DTOs.Inventory
{
    public class GetInventoryBatchDTO
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        public DateOnly ExpirationDate { get; set; }
    }
}
