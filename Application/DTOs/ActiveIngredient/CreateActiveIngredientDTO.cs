using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.ActiveIngredient
{
    public class CreateActiveIngredientDTO
    {
            [Required(ErrorMessage = "Active Ingredient Name is required.")]
            [StringLength(100, ErrorMessage = "Active Ingredient Name cannot exceed 100 characters.")]
            [Display (Name="Name")]    
            public string Name { get; set; } = null!;   
    }
}
