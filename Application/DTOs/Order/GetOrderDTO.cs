namespace Application.DTOs.Order
{
    public class GetOrderDTO
    {
        public int Id { get; set; }
        public int SupplierId { get; set; }
        public string? SupplierName { get; set; }
        public int UserId { get; set; }
        public string? UserName { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalValue { get; set; }
        public List<GetOrderItemDTO> OrderItems { get; set; } = new List<GetOrderItemDTO>();
    }
}