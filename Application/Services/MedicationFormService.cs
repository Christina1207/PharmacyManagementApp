using Application.DTOs.MedicationForm;
using Application.IServices.MedicationForm;
using AutoMapper;
using Domain.Entities;
using Domain.IRepositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Services
{
    public class MedicationFormService : IMedicationFormService
    {
        private readonly IRepository<MedicationForm, int> _medicationFormRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<MedicationFormService> _logger;

        public MedicationFormService(IRepository<MedicationForm, int> medicationFormRepository, IMapper mapper, ILogger<MedicationFormService> logger)
        {
            _medicationFormRepository = medicationFormRepository ?? throw new ArgumentNullException(nameof(medicationFormRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<GetMedicationFormDTO> CreateMedicationFormAsync(CreateMedicationFormDTO dto)
        {
            _logger.LogInformation("Attmpting to create medication form: {MedicationFormName}", dto.Name);
            try
            {
                if (dto is null)
                {
                    _logger.LogError("CreateMedicationFormAsync called with null DTO.");
                    throw new ArgumentNullException(nameof(dto), "Medication Form DTO cannot be null");
                }
                var existingMedicationForm = await _medicationFormRepository.GetByPredicateAsync(d => (dto.Name!.Equals(d.Name)));
                if (existingMedicationForm is not null)
                {
                    _logger.LogWarning("Medication Form already exists.Creation FAILED.");
                    throw new InvalidOperationException("Medication Form already exists.");

                }
                MedicationForm medicationform = _mapper.Map<MedicationForm>(dto);
                await _medicationFormRepository.AddAsync(medicationform);
                await _medicationFormRepository.SaveAsync();

                _logger.LogInformation("MedicationForm '{MedicationFormName}' (ID: {MedicationFormId}) created successfully.", medicationform.Name, medicationform.Id);
                return _mapper.Map<GetMedicationFormDTO>(medicationform);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while adding medication form with Name: {MedicationFormName}", dto.Name);
                throw;
            }
        }

        public async Task DeleteMedicationFormAsync(int id)
        {
            _logger.LogInformation("Attempting to delete medication form with ID: {MedicationFormId}", id);
            try
            {
                var medicationform = await _medicationFormRepository.GetByIdAsync(id);
                if (medicationform is null)
                {
                    _logger.LogWarning("Medication Form with ID: {MedicationFormId} not found for deletion.", id);
                    throw new KeyNotFoundException($"Medication Form with ID '{id}' not found.");
                }
                if (medicationform.Medications is not null && medicationform.Medications.Count != 0)
                {
                    _logger.LogWarning("Can't delete Medication Form with ID: {MedicationFormId} This Medication Form is a foriegn key in Medications.", id);
                    throw new DbUpdateException($"Medication Form with ID '{id}' is used by an medication or more.");
                }
                _medicationFormRepository.Delete(medicationform);
                await _medicationFormRepository.SaveAsync();

                _logger.LogInformation("Medication Form with ID: {MedicationFormId} deleted successfully.", id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting medication form with ID {Id}.", id);
                throw;
            }

        }

        public async Task<IEnumerable<GetMedicationFormDTO>> GetAllMedicationFormsAsync()
        {
            _logger.LogInformation("Attempting to retreive all medication forms ");
            try
            {
                var medicationforms = await _medicationFormRepository.GetAllAsync();
                _logger.LogInformation("Retreived all medication forms successfully");
                return _mapper.Map<IEnumerable<GetMedicationFormDTO>>(medicationforms);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occured while retreiving all medication forms");
                throw;
            }
        }

        public async Task<GetMedicationFormDTO> GetMedicationFormByIdAsync(int id)
        {
            _logger.LogInformation("Attempting to retrieve medication form with ID: {MedicationFormId}", id);
            try
            {
                var medicationform = await _medicationFormRepository.GetByIdAsync(id);

                if (medicationform is null)
                {
                    _logger.LogWarning("Medication Form with ID: {MedicationFormId} not found.", id);
                    throw new KeyNotFoundException($"Medication Form with ID '{id}' not found.");
                }

                _logger.LogInformation("Medication Form with ID: {MedicationFormId} retrieved successfully.", id);
                return _mapper.Map<GetMedicationFormDTO>(medicationform);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while retrieving medication form with id: {MedicationFormID}.", id);
                throw;
            }

        }

        public async Task UpdateMedicationFormAsync(UpdateMedicationFormDTO dto)
        {
            _logger.LogInformation("Attempting to update medication form with ID: {MedicationFormId}", dto.Id);
            if (dto is null)
            {
                _logger.LogWarning("UpdateMedicationFormAsync called with null DTO.");
                throw new ArgumentNullException(nameof(dto), "Medication Form update DTO cannot be null.");
            }
            try
            {
                var medicationform = await _medicationFormRepository.GetByIdAsync(dto.Id);
                if (medicationform is null)
                {
                    _logger.LogWarning("Medication Form with ID: {MedicationFormId} not found for update.", dto.Id);
                    throw new KeyNotFoundException($"Medication Form with ID '{dto.Id}' not found.");
                }

                // check for uniquness
                var sameMedicationForm = await _medicationFormRepository.GetByPredicateAsync(d => (dto.Name!.Equals(d.Name) ));
                if (sameMedicationForm is not null)
                {
                    _logger.LogWarning("Another medication form with the same name already exists. Update failed for ID: {MedicationFormId}.", dto.Id);
                    throw new InvalidOperationException("Another medication form with same attributes already exists.");
                }

                _mapper.Map(dto, medicationform);
                _medicationFormRepository.Update(medicationform);
                await _medicationFormRepository.SaveAsync();

                _logger.LogInformation("MedicationForm '{Name}' '{LastName}' updated successfully.", medicationform.Name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating medication form with ID {Id}.", dto.Id);
                throw;
            }
        }
    }
}
