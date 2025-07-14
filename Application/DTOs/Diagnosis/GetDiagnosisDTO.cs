using System.ComponentModel.DataAnnotations;


namespace Application.DTOs.Diagnosis
{
    public class GetDiagnosisDTO
    {
        [Display(Name = "Diagnosis Id")]
        public int Id { get; set; }

        [Display(Name = "Description")]
        public string? Description { get; set; }
    }
}
