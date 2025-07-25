using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Prescription
{
    public class CreatePrescriptionDTO
    {
        [Required]
        public int PatientId { get; set; }

        [Required]
        public int DoctorId { get; set; }

        [Required]
        public int DiagnosisId { get; set; }

        public DateOnly? IssueDate { get; set; } = DateOnly.FromDateTime(DateTime.Now);

        [Required]
        public int UserId { get; set; }

        [Required]
        [MinLength(1, ErrorMessage = "A prescription must have at least one medication.")]
        public List<PrescriptionItemDTO>? PrescriptionItems { get; set; }
    }
}
