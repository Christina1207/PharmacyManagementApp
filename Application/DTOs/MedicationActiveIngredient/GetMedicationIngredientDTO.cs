using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.MedicationActiveIngredient
{
    public class GetMedicationIngredientDTO
    {
        public int IngredientId { get; set; }
        public string? IngredientName { get; set; }
        public decimal Amount { get; set; }
    }
}
