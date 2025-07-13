using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.MedicationForm
{
    public class CreateMedicationFormDTO
    {
        [Required(ErrorMessage = "Name cannot be empty")]
        [Display(Name = "Name")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Unit cannot be empty")]
        [Display(Name = "Unit")]
        public string Unit { get; set; } = null!;

    }
}
