using Application.DTOs.MedicationClass;
using Application.IServices.MedicationClass;
using AutoMapper;
using Domain.Entities;
using Domain.IRepositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;


namespace Application.Services
{
    public class MedicationClassService:IMedicationClassService
    {
        private readonly IRepository<MedicationClass, int> _medicationClassRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<MedicationClassService> _logger;

        public MedicationClassService(IRepository<MedicationClass, int> medicationClassRepository, IMapper mapper, ILogger<MedicationClassService> logger)
        {
            _medicationClassRepository = medicationClassRepository ?? throw new ArgumentNullException(nameof(medicationClassRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<GetMedicationClassDTO> CreateMedicationClassAsync(CreateMedicationClassDTO dto)
        {
            _logger.LogInformation("Attmpting to create medication class: {MedicationClassName}", dto.Name);
            try
            {
                if (dto is null)
                {
                    _logger.LogError("CreateMedicationClassAsync called with null DTO.");
                    throw new ArgumentNullException(nameof(dto), "Medication Class DTO cannot be null");
                }
                var existingMedicationClass = await _medicationClassRepository.GetByPredicateAsync(d => (dto.Name!.Equals(d.Name)));
                if (existingMedicationClass is not null)
                {
                    _logger.LogWarning("Medication Class already exists.Creation FAILED.");
                    throw new InvalidOperationException("Medication Class already exists.");

                }
                MedicationClass medicationclass = _mapper.Map<MedicationClass>(dto);
                await _medicationClassRepository.AddAsync(medicationclass);
                await _medicationClassRepository.SaveAsync();

                _logger.LogInformation("MedicationClass '{MedicationClassName}' (ID: {MedicationClassId}) created successfully.", medicationclass.Name, medicationclass.Id);
                return _mapper.Map<GetMedicationClassDTO>(medicationclass);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while adding medication class with Name: {MedicationClassName}", dto.Name);
                throw;
            }
        }

        public async Task DeleteMedicationClassAsync(int id)
        {
            _logger.LogInformation("Attempting to delete medication class with ID: {MedicationClassId}", id);
            try
            {
                var medicationclass = await _medicationClassRepository.GetByIdAsync(id);
                if (medicationclass is null)
                {
                    _logger.LogWarning("Medication Class with ID: {MedicationClassId} not found for deletion.", id);
                    throw new KeyNotFoundException($"Medication Class with ID '{id}' not found.");
                }
                if (medicationclass.Medications is not null && medicationclass.Medications.Count != 0)
                {
                    _logger.LogWarning("Can't delete Medication Class with ID: {MedicationClassId} This Medication Class is a foriegn key in Medications.", id);
                    throw new DbUpdateException($"Medication Class with ID '{id}' is used by an medication or more.");
                }
                _medicationClassRepository.Delete(medicationclass);
                await _medicationClassRepository.SaveAsync();

                _logger.LogInformation("Medication Class with ID: {MedicationClassId} deleted successfully.", id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting medication class with ID {Id}.", id);
                throw;
            }

        }

        public async Task<IEnumerable<GetMedicationClassDTO>> GetAllMedicationClassesAsync()
        {
            _logger.LogInformation("Attempting to retreive all medication classes ");
            try
            {
                var medicationclasses = await _medicationClassRepository.GetAllAsync();
                _logger.LogInformation("Retreived all medication classes successfully");
                return _mapper.Map<IEnumerable<GetMedicationClassDTO>>(medicationclasses);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occured while retreiving all medication classes");
                throw;
            }
        }

        public async Task<GetMedicationClassDTO> GetMedicationClassByIdAsync(int id)
        {
            _logger.LogInformation("Attempting to retrieve medication class with ID: {MedicationClassId}", id);
            try
            {
                var medicationclass = await _medicationClassRepository.GetByIdAsync(id);

                if (medicationclass is null)
                {
                    _logger.LogWarning("Medication Class with ID: {MedicationClassId} not found.", id);
                    throw new KeyNotFoundException($"Medication Class with ID '{id}' not found.");
                }

                _logger.LogInformation("Medication Class with ID: {MedicationClassId} retrieved successfully.", id);
                return _mapper.Map<GetMedicationClassDTO>(medicationclass);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while retrieving medication class with id: {MedicationClassID}.", id);
                throw;
            }

        }

        public async Task UpdateMedicationClassAsync(UpdateMedicationClassDTO dto)
        {
            _logger.LogInformation("Attempting to update medication class with ID: {MedicationClassId}", dto.Id);
            if (dto is null)
            {
                _logger.LogWarning("UpdateMedicationClassAsync called with null DTO.");
                throw new ArgumentNullException(nameof(dto), "Medication Class update DTO cannot be null.");
            }
            try
            {
                var medicationclass = await _medicationClassRepository.GetByIdAsync(dto.Id);
                if (medicationclass is null)
                {
                    _logger.LogWarning("Medication Class with ID: {MedicationClassId} not found for update.", dto.Id);
                    throw new KeyNotFoundException($"Medication Class with ID '{dto.Id}' not found.");
                }

                // check for uniquness
                var sameMedicationClass = await _medicationClassRepository.GetByPredicateAsync(d => (dto.Name!.Equals(d.Name)));
                if (sameMedicationClass is not null)
                {
                    _logger.LogWarning("Another medication class with the same name already exists. Update failed for ID: {MedicationClassId}.", dto.Id);
                    throw new InvalidOperationException("Another medication class with same attributes already exists.");
                }

                _mapper.Map(dto, medicationclass);
                _medicationClassRepository.Update(medicationclass);
                await _medicationClassRepository.SaveAsync();

                _logger.LogInformation("MedicationClass '{Name}' updated successfully.", medicationclass.Name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating medication class with ID {Id}.", dto.Id);
                throw;
            }
        }

    }
}
