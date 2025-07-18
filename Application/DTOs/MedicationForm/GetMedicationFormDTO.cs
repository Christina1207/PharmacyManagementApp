using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.MedicationForm
{
    public class GetMedicationFormDTO
    {

        [Display(Name = "Medication Form ID")]
        public int Id { get; set; }
        
        [Display(Name = "Name")]
        public string Name { get; set; } = null!;

        [Display(Name = "Unit")]
        public string Unit { get; set; } = null!;
    }
}
