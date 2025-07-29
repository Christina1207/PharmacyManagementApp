namespace Application.DTOs.Order
{
    public class GetOrderItemDTO
    {
        public int Id { get; set; }
        public int MedicationId { get; set; }
        public string? MedicationName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public DateOnly ExpirationDate { get; set; }
    }
}
