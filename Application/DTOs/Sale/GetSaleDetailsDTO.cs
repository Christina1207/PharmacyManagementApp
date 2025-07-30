using Application.DTOs.Prescription;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Sale
{
    public class GetSaleDetailsDTO
    {
        public int Id { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal Discount { get; set; }
        public decimal AmountReceived { get; set; }
        public string? PharmacistName { get; set; }
        public DateTime DispenseDate { get; set; }

        // Nested Prescription Details
        public GetPrescriptionDTO? Prescription { get; set; }
    }
}
