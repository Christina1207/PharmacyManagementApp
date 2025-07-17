using Application.DTOs.Diagnosis;
using Application.IServices.Diagnosis;
using AutoMapper;
using Domain.Entities;
using Domain.IRepositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Services
{
    public class DiagnosisService : IDiagnosisService
    {
        private readonly IRepository<Diagnosis, int> _diagnosisRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<DiagnosisService> _logger;

        public DiagnosisService(IRepository<Diagnosis, int> diagnosisRepository, IMapper mapper, ILogger<DiagnosisService> logger)
        {
            _diagnosisRepository = diagnosisRepository ?? throw new ArgumentNullException(nameof(diagnosisRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<GetDiagnosisDTO> CreateDiagnosisAsync(CreateDiagnosisDTO dto)
        {
            _logger.LogInformation("Attmpting to create diagnosis: {DiagnosisDescription}", dto.Description);
            try
            {
                if (dto is null)
                {
                    _logger.LogError("CreateDiagnosisAsync called with null DTO.");
                    throw new ArgumentNullException(nameof(dto), "Diagnosis DTO cannot be null");
                }
                var existingDiagnosis = await _diagnosisRepository.GetByPredicateAsync(s => dto.Description!.Equals(s.Description));
                if (existingDiagnosis is not null)
                {
                    _logger.LogWarning("Diagnosis with description {DiagnosisDescription} alreay exists.Creation FAILED.", dto.Description);
                    throw new InvalidOperationException($"Diagnosis with description '{dto.Description}' already exists.");

                }
                Diagnosis diagnosis = _mapper.Map<Diagnosis>(dto);
                await _diagnosisRepository.AddAsync(diagnosis);
                await _diagnosisRepository.SaveAsync();

                _logger.LogInformation("Diagnosis '{DiagnosisDescription}' (ID: {DiagnosisId}) created successfully.", diagnosis.Description, diagnosis.Id);
                return _mapper.Map<GetDiagnosisDTO>(diagnosis);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while adding diagnosis with Description: {DiagnosisDescription}", dto.Description);
                throw;
            }
        }

        public async Task DeleteDiagnosisAsync(int id)
        {
            _logger.LogInformation("Attempting to delete diagnosis with ID: {DiagnosisId}", id);
            try
            {
                var diagnosis = await _diagnosisRepository.GetByIdAsync(id);
                if (diagnosis is null)
                {
                    _logger.LogWarning("Diagnosis with ID: {DiagnosisId} not found for deletion.", id);
                    throw new KeyNotFoundException($"Diagnosis with ID '{id}' not found.");
                }
                if (diagnosis.Prescriptions is not null && diagnosis.Prescriptions.Count != 0)
                {
                    _logger.LogWarning("Can't delete Diagnosis with ID: {DiagnosisId} This Diagnosis is a foriegn key in Prescription.", id);
                    throw new DbUpdateException($"Diagnosis with ID '{id}' is used by an prescription or more.");
                }
                _diagnosisRepository.Delete(diagnosis);
                await _diagnosisRepository.SaveAsync();

                _logger.LogInformation("Diagnosis with ID: {DiagnosisId} deleted successfully.", id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting diagnosis with ID {Id}.", id);
                throw;
            }

        }

        public async Task<IEnumerable<GetDiagnosisDTO>> GetAllDiagnosesAsync()
        {
            _logger.LogInformation("Attempting to retreive all diagnoses ");
            try
            {
                var diagnoses = await _diagnosisRepository.GetAllAsync();
                _logger.LogInformation("Retreived all diagnoses successfully");
                return _mapper.Map<IEnumerable<GetDiagnosisDTO>>(diagnoses);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occured while retreiving all diagnoses");
                throw;
            }
        }

        public async Task<GetDiagnosisDTO> GetDiagnosisByIdAsync(int id)
        {
            _logger.LogInformation("Attempting to retrieve diagnosis with ID: {DiagnosisId}", id);
            try
            {
                var diagnosis = await _diagnosisRepository.GetByIdAsync(id);

                if (diagnosis is null)
                {
                    _logger.LogWarning("Diagnosis with ID: {DiagnosisId} not found.", id);
                    throw new KeyNotFoundException($"Diagnosis with ID '{id}' not found.");
                }

                _logger.LogInformation("Diagnosis with ID: {DiagnosisId} retrieved successfully.", id);
                return _mapper.Map<GetDiagnosisDTO>(diagnosis);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while retrieving diagnosis with id: {DiagnosisID}.", id);
                throw;
            }

        }

        public async Task UpdateDiagnosisAsync(UpdateDiagnosisDTO dto)
        {
            _logger.LogInformation("Attempting to update diagnosis with ID: {DiagnosisId}", dto.Id);
            if (dto is null)
            {
                _logger.LogWarning("UpdateDiagnosisAsync called with null DTO.");
                throw new ArgumentNullException(nameof(dto), "Diagnosis update DTO cannot be null.");
            }
            try
            {
                var diagnosis = await _diagnosisRepository.GetByIdAsync(dto.Id);
                if (diagnosis is null)
                {
                    _logger.LogWarning("Diagnosis with ID: {DiagnosisId} not found for update.", dto.Id);
                    throw new KeyNotFoundException($"Diagnosis with ID '{dto.Id}' not found.");
                }

                // if description is updated , we check for uniqueness
                if (!diagnosis.Description!.Equals(dto.Description, StringComparison.OrdinalIgnoreCase))
                {
                    var diagnosisWithSameDescription = await _diagnosisRepository.GetByPredicateAsync(c => c.Description!.Equals(dto.Description) && c.Id != dto.Id);
                    if (diagnosisWithSameDescription is not null)
                    {
                        _logger.LogWarning("Another diagnosis with description '{DiagnosisDescription}' already exists. Update failed for ID: {DiagnosisId}.", dto.Description, dto.Id);
                        throw new InvalidOperationException($"Another diagnosis with description '{dto.Description}' already exists.");
                    }
                }
                _mapper.Map(dto, diagnosis);
                _diagnosisRepository.Update(diagnosis);
                await _diagnosisRepository.SaveAsync();

                _logger.LogInformation("Diagnosis '{Description}' updated successfully.", diagnosis.Description);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating diagnosis with ID {Id}.", dto.Id);
                throw;
            }
        }
    }
}
