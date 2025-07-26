using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Prescription
{
    public class GetPrescriptionDTO
    {
        public int Id { get; set; }
        public decimal TotalValue { get; set; }
        public DateTime DispenseDate { get; set; }
        public DateOnly? IssueDate { get; set; }
        public string? PatientName { get; set; }
        public string? DoctorName { get; set; }
        public string? PharmacistName { get; set; }
        public List<GetPrescriptionItemDTO>? Items { get; set; }
    }
}
