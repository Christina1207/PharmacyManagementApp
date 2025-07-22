using System.ComponentModel.DataAnnotations;


namespace Application.DTOs.MedicationClass
{
    public class CreateMedicationClassDTO
    {
        [Required]
        [StringLength(50)]
        public string? Name { get; set; }

        [StringLength(255)]
        public string? Description { get; set; }
    }
}
