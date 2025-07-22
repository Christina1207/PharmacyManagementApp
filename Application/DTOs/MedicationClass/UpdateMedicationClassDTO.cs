using System.ComponentModel.DataAnnotations;


namespace Application.DTOs.MedicationClass
{
    public class UpdateMedicationClassDTO
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string? Name { get; set; }

        [StringLength(255)]
        public string? Description { get; set; }
    }
}
