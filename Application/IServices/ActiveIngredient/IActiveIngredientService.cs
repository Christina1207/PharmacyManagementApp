using Application.DTOs.ActiveIngredient;

namespace Application.IServices.ActiveIngredient
{
    public interface IActiveIngredientService
    {
        public Task<GetActiveIngredientDTO> CreateActiveIngredientAsync(CreateActiveIngredientDTO dto);

        public Task UpdateActiveIngredientAsync(UpdateActiveIngredientDTO dto);
        public Task DeleteActiveIngredientAsync(int id);
        Task<IEnumerable<GetActiveIngredientDTO>> GetAllActiveIngredientsAsync();
        Task<GetActiveIngredientDTO> GetActiveIngredientByIdAsync(int id);

    }
}
