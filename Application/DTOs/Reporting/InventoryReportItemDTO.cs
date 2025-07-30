using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Reporting
{
    public class InventoryReportItemDTO
    {
        public int MedicationId { get; set; }
        public string MedicationName { get; set; } = string.Empty;
        public int TotalQuantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalValue => TotalQuantity * UnitPrice;
        public bool IsBelowMinStock { get; set; }
        public int DaysUntilEarliestExpiry { get; set; }
    }

    public class InventoryReportDTO
    {
        public DateTime GeneratedAt { get; set; }
        public int TotalItemsInStock { get; set; }
        public int LowStockItemsCount { get; set; }
        public decimal TotalInventoryValue { get; set; }
        public List<InventoryReportItemDTO> Items { get; set; } = new List<InventoryReportItemDTO>();
    }
}
