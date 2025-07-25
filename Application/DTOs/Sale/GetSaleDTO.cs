using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Sale
{
    public class GetSaleDTO
    {
        public int Id { get; set; }
        public int PrescriptionId { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal Discount { get; set; }
        public decimal AmountReceived { get; set; }
        public string? PharmacistName { get; set; }
    }
}
