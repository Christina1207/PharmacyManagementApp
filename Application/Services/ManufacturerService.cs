using Application.DTOs.Manufacturer;
using Application.IServices.Manufacturer;
using AutoMapper;
using Domain.Entities;
using Domain.IRepositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;


namespace Application.Services
{
    public class ManufacturerService : IManufacturerService
    {
        private readonly IRepository<Manufacturer, int> _manufacturerRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<ManufacturerService> _logger;

        public ManufacturerService(IRepository<Manufacturer, int> manufacturerRepository, IMapper mapper, ILogger<ManufacturerService> logger)
        {
            _manufacturerRepository = manufacturerRepository ?? throw new ArgumentNullException(nameof(manufacturerRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<GetManufacturerDTO> CreateManufacturerAsync(CreateManufacturerDTO dto)
        {
            _logger.LogInformation("Attmpting to create manufacturer: {ManufacturerName}", dto.Name);
            try
            {
                if (dto is null)
                {
                    _logger.LogError("CreateManufacturerAsync called with null DTO.");
                    throw new ArgumentNullException(nameof(dto), "Manufacturer DTO cannot be null");
                }
                var existingManufacturer = await _manufacturerRepository.GetByPredicateAsync(s => dto.Name!.Equals(s.Name));
                if (existingManufacturer is not null)
                {
                    _logger.LogWarning("Manufacturer with name {ManufacturerName} alreay exists.Creation FAILED.", dto.Name);
                    throw new InvalidOperationException($"Manufacturer with name '{dto.Name}' already exists.");

                }
                Manufacturer manufacturer = _mapper.Map<Manufacturer>(dto);
                await _manufacturerRepository.AddAsync(manufacturer);
                await _manufacturerRepository.SaveAsync();

                _logger.LogInformation("Manufacturer '{ManufacturerName}' (ID: {ManufacturerId}) created successfully.", manufacturer.Name, manufacturer.Id);
                return _mapper.Map<GetManufacturerDTO>(manufacturer);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while adding manufacturer with Name: {ManufacturerName}", dto.Name);
                throw;
            }
        }

        public async Task DeleteManufacturerAsync(int id)
        {
            _logger.LogInformation("Attempting to delete manufacturer with ID: {ManufacturerId}", id);
            try
            {
                var manufacturer = await _manufacturerRepository.GetByIdAsync(id);
                if (manufacturer is null)
                {
                    _logger.LogWarning("Manufacturer with ID: {ManufacturerId} not found for deletion.", id);
                    throw new KeyNotFoundException($"Manufacturer with ID '{id}' not found.");
                }
                if (manufacturer.Medications is not null && manufacturer.Medications.Count != 0)
                {
                    _logger.LogWarning("Can't delete Manufacturer with ID: {ManufacturerId} This Manufacturer is a foriegn key in Medications.", id);
                    throw new DbUpdateException($"Manufacturer with ID '{id}' is used by a Medication or more.");
                }
                _manufacturerRepository.Delete(manufacturer);
                await _manufacturerRepository.SaveAsync();

                _logger.LogInformation("Manufacturer with ID: {ManufacturerId} deleted successfully.", id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting manufacturer with ID {Id}.", id);
                throw;
            }

        }

        public async Task<IEnumerable<GetManufacturerDTO>> GetAllManufacturersAsync()
        {
            _logger.LogInformation("Attempting to retreive all manufacturers ");
            try
            {
                var manufacturers = await _manufacturerRepository.GetAllAsync();
                _logger.LogInformation("Retreived all manufacturers successfully");
                return _mapper.Map<IEnumerable<GetManufacturerDTO>>(manufacturers);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occured while retreiving all manufacturers");
                throw;
            }
        }

        public async Task<GetManufacturerDTO> GetManufacturerByIdAsync(int id)
        {
            _logger.LogInformation("Attempting to retrieve manufacturer with ID: {ManufacturerId}", id);
            try
            {
                var manufacturer = await _manufacturerRepository.GetByIdAsync(id);

                if (manufacturer is null)
                {
                    _logger.LogWarning("Manufacturer with ID: {ManufacturerId} not found.", id);
                    throw new KeyNotFoundException($"Manufacturer with ID '{id}' not found.");
                }

                _logger.LogInformation("Manufacturer with ID: {ManufacturerId} retrieved successfully.", id);
                return _mapper.Map<GetManufacturerDTO>(manufacturer);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while retrieving manufacturer with id: {ManufacturerID}.", id);
                throw;
            }

        }

        public async Task UpdateManufacturerAsync(UpdateManufacturerDTO dto)
        {
            _logger.LogInformation("Attempting to update manufacturer with ID: {ManufacturerId}", dto.Id);
            if (dto is null)
            {
                _logger.LogWarning("UpdateManufacturerAsync called with null DTO.");
                throw new ArgumentNullException(nameof(dto), "Manufacturer update DTO cannot be null.");
            }
            try
            {
                var manufacturer = await _manufacturerRepository.GetByIdAsync(dto.Id);
                if (manufacturer is null)
                {
                    _logger.LogWarning("Manufacturer with ID: {ManufacturerId} not found for update.", dto.Id);
                    throw new KeyNotFoundException($"Manufacturer with ID '{dto.Id}' not found.");
                }

                // if name is updated , we check for uniqueness
                if (!manufacturer.Name!.Equals(dto.Name, StringComparison.OrdinalIgnoreCase))
                {
                    var manufacturerWithSameName = await _manufacturerRepository.GetByPredicateAsync(c => c.Name!.Equals(dto.Name) && c.Id != dto.Id);
                    if (manufacturerWithSameName is not null)
                    {
                        _logger.LogWarning("Another manufacturer with name '{ManufacturerName}' already exists. Update failed for ID: {ManufacturerId}.", dto.Name, dto.Id);
                        throw new InvalidOperationException($"Another manufacturer with name '{dto.Name}' already exists.");
                    }
                }
                _mapper.Map(dto, manufacturer);
                _manufacturerRepository.Update(manufacturer);
                await _manufacturerRepository.SaveAsync();

                _logger.LogInformation("Manufacturer '{Name}' updated successfully.", manufacturer.Name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating manufacturer with ID {Id}.", dto.Id);
                throw;
            }
        }
    }


}
