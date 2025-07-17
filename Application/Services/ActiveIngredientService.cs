using Application.DTOs.ActiveIngredient;
using Application.IServices.ActiveIngredient;
using AutoMapper;
using Domain.Entities;
using Domain.IRepositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Services
{
    public class ActiveIngredientService:IActiveIngredientService
    {
        private readonly IRepository<ActiveIngredient, int> _activeingredientRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<ActiveIngredientService> _logger;

        public ActiveIngredientService(IRepository<ActiveIngredient, int> activeingredientRepository, IMapper mapper, ILogger<ActiveIngredientService> logger)
        {
            _activeingredientRepository = activeingredientRepository ?? throw new ArgumentNullException(nameof(activeingredientRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<GetActiveIngredientDTO> CreateActiveIngredientAsync(CreateActiveIngredientDTO dto)
        {
            _logger.LogInformation("Attmpting to create Active ingredient: {ActiveIngredientName}", dto.Name);
            try
            {
                if (dto is null)
                {
                    _logger.LogError("CreateActiveIngredientAsync called with null DTO.");
                    throw new ArgumentNullException(nameof(dto), "Active Ingredient DTO cannot be null");
                }
                var existingActiveIngredient = await _activeingredientRepository.GetByPredicateAsync(s => dto.Name!.Equals(s.Name));
                if (existingActiveIngredient is not null)
                {
                    _logger.LogWarning("Active Ingredient with name {ActiveIngredientName} alreay exists.Creation FAILED.", dto.Name);
                    throw new InvalidOperationException($"Active Ingredient with name '{dto.Name}' already exists.");

                }
                ActiveIngredient activeingredient = _mapper.Map<ActiveIngredient>(dto);
                await _activeingredientRepository.AddAsync(activeingredient);
                await _activeingredientRepository.SaveAsync();

                _logger.LogInformation("Active Ingredient '{ActiveIngredientName}' (ID: {ActiveIngredientId}) created successfully.", activeingredient.Name, activeingredient.Id);
                return _mapper.Map<GetActiveIngredientDTO>(activeingredient);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while adding active ingredient with Name: {ActiveIngredientName}", dto.Name);
                throw;
            }
        }

        public async Task DeleteActiveIngredientAsync(int id)
        {
            _logger.LogInformation("Attempting to delete active ingredient with ID: {ActiveIngredientId}", id);
            try
            {
                var activeingredient = await _activeingredientRepository.GetByIdAsync(id);
                if (activeingredient is null)
                {
                    _logger.LogWarning("Active Ingredient with ID: {ActiveIngredientId} not found for deletion.", id);
                    throw new KeyNotFoundException($"Active Ingredient with ID '{id}' not found.");
                }
                if (activeingredient.MedicationActiveIngredients is not null && activeingredient.MedicationActiveIngredients.Count != 0)
                {
                    _logger.LogWarning("Can't delete Active Ingredient with ID: {ActiveIngredientId} This Active Ingredient is a foriegn key in Medication.", id);
                    throw new DbUpdateException($"Active Ingredient with ID '{id}' is used by an medication or more.");
                }
                _activeingredientRepository.Delete(activeingredient);
                await _activeingredientRepository.SaveAsync();

                _logger.LogInformation("Active Ingredient with ID: {ActiveIngredientId} deleted successfully.", id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting active ingredient with ID {Id}.", id);
                throw;
            }

        }

        public async Task<IEnumerable<GetActiveIngredientDTO>> GetAllActiveIngredientsAsync()
        {
            _logger.LogInformation("Attempting to retreive all active ingredients ");
            try
            {
                var activeingredients = await _activeingredientRepository.GetAllAsync();
                _logger.LogInformation("Retreived all active ingredients successfully");
                return _mapper.Map<IEnumerable<GetActiveIngredientDTO>>(activeingredients);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occured while retreiving all active ingredients");
                throw;
            }
        }

        public async Task<GetActiveIngredientDTO> GetActiveIngredientByIdAsync(int id)
        {
            _logger.LogInformation("Attempting to retrieve active ingredient with ID: {ActiveIngredientId}", id);
            try
            {
                var activeingredient = await _activeingredientRepository.GetByIdAsync(id);

                if (activeingredient is null)
                {
                    _logger.LogWarning("Active Ingredient with ID: {ActiveIngredientId} not found.", id);
                    throw new KeyNotFoundException($"Active Ingredient with ID '{id}' not found.");
                }

                _logger.LogInformation("Active Ingredient with ID: {ActiveIngredientId} retrieved successfully.", id);
                return _mapper.Map<GetActiveIngredientDTO>(activeingredient);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while retrieving active ingredient with id: {ActiveIngredientID}.", id);
                throw;
            }

        }

        public async Task UpdateActiveIngredientAsync(UpdateActiveIngredientDTO dto)
        {
            _logger.LogInformation("Attempting to update active ingredient with ID: {ActiveIngredientId}", dto.Id);
            if (dto is null)
            {
                _logger.LogWarning("UpdateActiveIngredientAsync called with null DTO.");
                throw new ArgumentNullException(nameof(dto), "Active Ingredient update DTO cannot be null.");
            }
            try
            {
                var activeingredient = await _activeingredientRepository.GetByIdAsync(dto.Id);
                if (activeingredient is null)
                {
                    _logger.LogWarning("Active Ingredient with ID: {ActiveIngredientId} not found for update.", dto.Id);
                    throw new KeyNotFoundException($"Active Ingredient with ID '{dto.Id}' not found.");
                }

                // if name is updated , we check for uniqueness
                if (!activeingredient.Name!.Equals(dto.Name, StringComparison.OrdinalIgnoreCase))
                {
                    var activeingredientWithSameName = await _activeingredientRepository.GetByPredicateAsync(c => c.Name!.Equals(dto.Name) && c.Id != dto.Id);
                    if (activeingredientWithSameName is not null)
                    {
                        _logger.LogWarning("Another active ingredient with name '{ActiveIngredientName}' already exists. Update failed for ID: {ActiveIngredientId}.", dto.Name, dto.Id);
                        throw new InvalidOperationException($"Another active ingredient with name '{dto.Name}' already exists.");
                    }
                }
                _mapper.Map(dto, activeingredient);
                _activeingredientRepository.Update(activeingredient);
                await _activeingredientRepository.SaveAsync();

                _logger.LogInformation("Active Ingredient '{Name}' updated successfully.", activeingredient.Name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating active ingredient with ID {Id}.", dto.Id);
                throw;
            }
        }
    }
}
