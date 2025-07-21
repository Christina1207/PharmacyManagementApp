using Application.DTOs.InsuredPerson;
using Application.IServices.InsuredPerson;
using AutoMapper;
using Domain.Entities;
using Domain.IRepositories;
using Microsoft.Extensions.Logging;

namespace Application.Services
{
    public class InsuredPersonService :IInsuredPersonService
    {
        private readonly IRepository<InsuredPerson,int> _repository;
        private readonly IMapper _mapper;
        private readonly ILogger<InsuredPersonService> _logger;

        public InsuredPersonService(
            IRepository<InsuredPerson, int> repository, IMapper mapper, ILogger<InsuredPersonService> logger)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<GetInsuredPersonDTO> CreateInsuredPersonAsync(CreateInsuredPersonDTO createInsuredPersonDto)
        {
            if (createInsuredPersonDto is null)
            {
                _logger.LogError("CreateInsuredPersonAsync: Input DTO is null.");
                throw new ArgumentNullException(nameof(createInsuredPersonDto), "Insured Person creation DTO cannot be null.");
            }
            _logger.LogInformation("Attempting to create base insured person for a insuredperson");

            try
            {

                var insuredpersonEntity = _mapper.Map<InsuredPerson>(createInsuredPersonDto);
                await _repository.AddAsync(insuredpersonEntity);
                await _repository.SaveAsync();

                _logger.LogInformation("Insured Person '{Id}' created successfully.", insuredpersonEntity.Id);
                return _mapper.Map<GetInsuredPersonDTO>(insuredpersonEntity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating an instance of insured person.");
                throw;
            }
        }

        public async Task<GetInsuredPersonDTO> GetInsuredPersonByIdAsync(int id)
        {
            _logger.LogInformation("Attempting to retrieve insured person with ID: {Id}", id);
            try
            {
                var insuredperson = await _repository.GetByIdAsync(id);
                if (insuredperson is null)
                {
                    _logger.LogWarning("Insured Person with ID {Id} was not found.", id);
                    throw new KeyNotFoundException($"Insured Person with ID {id} was not found.");
                }

                _logger.LogDebug("Insured Person '{Id}' retrieved successfully.", id);
                return _mapper.Map<GetInsuredPersonDTO>(insuredperson);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving insuredperson with ID {Id}.", id);
                throw;
            }
        }

        public async Task<IEnumerable<GetInsuredPersonDTO>> GetAllInsuredPersonsAsync()
        {
            _logger.LogInformation("Attempting to retrieve all insured persons.");
            try
            {
                var insuredpersons = await _repository.GetAllAsync();
                _logger.LogDebug("{Count} insured persons retrieved.", insuredpersons?.Count() ?? 0);
                return _mapper.Map<IEnumerable<GetInsuredPersonDTO>>(insuredpersons);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving all insured persons.");
                throw;
            }
        }

        public async Task DeleteInsuredPersonAsync(int id)
        {
            try
            {
                var insuredperson = await _repository.GetByIdAsync(id)
                    ?? throw new KeyNotFoundException($"Insured Person with ID {id} was not found.");

                await _repository.DeleteByIdAsync(id);
                await _repository.SaveAsync().ConfigureAwait(false);

                _logger.LogInformation("Insured Person '{Id}' deleted successfully.", id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting insured person with ID {Id}.", id);
                throw;
            }
        }

        private async Task SetInsuredPersonStatusAsync(int Id, string status)
        {
            _logger.LogInformation("Setting status to '{Status}' for insuredperson ID: {InsuredPersonId}", status, Id);
            var insuredperson = await _repository.GetByIdAsync(Id);
            if (insuredperson == null)
            {
                _logger.LogWarning("InsuredPerson with ID {InsuredPersonId} not found for status update.", Id);
                throw new KeyNotFoundException($"InsuredPerson with ID {Id} not found.");
            }

            insuredperson.Status = status;
            _repository.Update(insuredperson);
            await _repository.SaveAsync();
            _logger.LogInformation("Successfully updated status for insuredperson ID: {InsuredPersonId}", Id);
        }

        public async Task ActivateInsuredPersonAsync(int id)
        {
            await SetInsuredPersonStatusAsync(id, "Active");
        }

        public async Task DeactivateInsuredPersonAsync(int id)
        {
            await SetInsuredPersonStatusAsync(id, "Inactive");
        }
    }
}
