using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.ActiveIngredient
{
    public class GetActiveIngredientDTO
    {
        [Display(Name ="Ingredient ID")]
        public int Id { get; set; }
        [Display(Name="Name")]
        public string Name { get; set; } = null!;
    }
}
