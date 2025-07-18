using System.ComponentModel.DataAnnotations;

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
