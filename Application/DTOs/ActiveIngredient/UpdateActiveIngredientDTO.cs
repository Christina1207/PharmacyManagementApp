using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.ActiveIngredient
{
    public class UpdateActiveIngredientDTO
    {
        [Required(ErrorMessage = "Active Ingredient ID is required for update.")]
        [Display(Name = "Ingredient ID")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Active Ingredient Name is required.")]
        [StringLength(100, ErrorMessage = "Active Ingredient Name cannot exceed 100 characters.")]
        [Display(Name = "Name")]
        public string Name { get; set; } = null!;
    }
}
