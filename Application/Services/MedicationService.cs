using Application.DTOs.Doctor;
using Application.DTOs.Medication;
using Application.IServices.Medication;
using AutoMapper;
using Domain.Entities;
using Domain.IRepositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Numerics;

namespace Application.Services
{
    public class MedicationService:IMedicationService
    {
        private readonly IRepository<Medication, int> _medicationRepository;
        private readonly IRepository<MedicationActiveIngredient, int> _medicationIngredientRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<MedicationService> _logger;

        public MedicationService(IRepository<Medication, int> medicationRepository, IMapper mapper, ILogger<MedicationService> logger, IRepository<MedicationActiveIngredient, int> medicationIngredientRepository)
        {
            _medicationRepository = medicationRepository;
            _mapper = mapper;
            _logger = logger;
            _medicationIngredientRepository = medicationIngredientRepository;
          
        }

        public async Task<GetMedicationDTO> CreateMedicationAsync(CreateMedicationDTO dto)
        {
            _logger.LogInformation("Attempting to create new medication: {Name}", dto.Name);
            try
            {
                if (dto == null)
                {
                    _logger.LogError("CreateMedicationtAsync called with null DTO.");
                    throw new ArgumentNullException(nameof(dto), "Medication DTO cannot be null");
                }

                var existing = await _medicationRepository.GetByPredicateAsync(m => m.Barcode == dto.Barcode);
                if (existing != null)
                {
                    _logger.LogWarning("A medication with barcode {Barcode} already exists.", dto.Barcode);
                    throw new InvalidOperationException($"A medication with barcode '{dto.Barcode}' already exists.");
                }

                var medication = _mapper.Map<Medication>(dto);
                await _medicationRepository.AddAsync(medication);
                await _medicationRepository.SaveAsync();

                _logger.LogInformation("Successfully created medication with ID: {Id}", medication.Id);
                //return await GetMedicationByIdAsync(medication.Id);
                return _mapper.Map<GetMedicationDTO>(medication);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while adding medication with Name: {’MedicationName}", dto.Name);
                throw;
            }
        }

        public async Task UpdateMedicationAsync(UpdateMedicationDTO dto)
        {
            _logger.LogInformation("Attempting to update medication ID: {Id}", dto.Id);
            var medication = await _medicationRepository.GetByIdAsync(dto.Id);
            if (medication == null)
            {
                _logger.LogWarning("Medication with ID {Id} not found for update.", dto.Id);
                throw new KeyNotFoundException($"Medication with ID {dto.Id} not found.");
            }

            // Update main medication properties
            _mapper.Map(dto, medication);

            // Handle the complex logic of updating ingredients
            var existingIngredients = medication.MedicationActiveIngredients.ToList();
            var incomingIngredients = _mapper.Map<List<MedicationActiveIngredient>>(dto.ActiveIngredients);

            // Remove ingredients that are no longer in the list
            foreach (var existing in existingIngredients)
            {
                if (!incomingIngredients.Any(i => i.IngredientId == existing.IngredientId))
                {
                    _medicationIngredientRepository.Delete(existing);
                }
            }

            // Add or Update ingredients
            foreach (var incoming in incomingIngredients)
            {
                var existing = existingIngredients.FirstOrDefault(i => i.IngredientId == incoming.IngredientId);
                if (existing != null)
                {
                    // Update amount
                    existing.Amount = incoming.Amount;
                }
                else
                {
                    // Add new ingredient
                    medication.MedicationActiveIngredients.Add(new MedicationActiveIngredient
                    {
                        MedicationId = medication.Id,
                        IngredientId = incoming.IngredientId,
                        Amount = incoming.Amount
                    });
                }
            }

            _medicationRepository.Update(medication);
            await _medicationRepository.SaveAsync();
            _logger.LogInformation("Successfully updated medication ID: {Id}", dto.Id);
        }


        public async Task<IEnumerable<GetMedicationDTO>> GetAllMedicationsAsync()
        {
            _logger.LogInformation("Retrieving all medications.");
            try
            {
                var medications = await _medicationRepository.GetAllAsync();
                return _mapper.Map<IEnumerable<GetMedicationDTO>>(medications);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error occured while retreiving all medications");
                throw;
            }
        }

        public async Task<GetMedicationDTO> GetMedicationByIdAsync(int id)
        {
            _logger.LogInformation("Retrieving medication by ID: {Id}", id);
            try
            {
                var medication = await _medicationRepository.GetByIdAsync(id);
                if (medication == null)
                {
                    _logger.LogWarning("Medication with ID {Id} not found.", id);
                    throw new KeyNotFoundException($"Medication with ID {id} not found.");
                }
                return _mapper.Map<GetMedicationDTO>(medication);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while retrieving medication with id: {MedicationID}.", id);
                throw;
            }
        }

        public async Task DeleteMedicationAsync(int id)
        {
            _logger.LogInformation("Attempting to delete medication ID: {Id}", id);
            var medication = await _medicationRepository.GetByIdAsync(id);
            if (medication == null)
            {
                _logger.LogWarning("Medication with ID {Id} not found for deletion.", id);
                throw new KeyNotFoundException($"Medication with ID {id} not found.");
            }

            // BUSINESS RULE: Cannot delete a medication if it is part of any inventory or prescription history.
            if (medication.InventoryItems.Count != 0 || medication.PrescriptionItems.Count != 0)
            {
                _logger.LogWarning("Attempted to delete medication ID {Id}, which has inventory or prescription history.", id);
                throw new DbUpdateException("This medication cannot be deleted because it is linked to inventory or prescription records.");
            }

            // Delete associated ingredients first
            foreach (var ingredient in medication.MedicationActiveIngredients.ToList())
            {
                _medicationIngredientRepository.Delete(ingredient);
            }

            _medicationRepository.Delete(medication);
            await _medicationRepository.SaveAsync();
            _logger.LogInformation("Successfully deleted medication ID: {Id}", id);
        }

    }
}
