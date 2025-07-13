using Application.DTOs.Supplier;
using Application.IServices;
using AutoMapper;
using Domain.Entities;
using Domain.IRepositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
namespace Application.Services
{
    public class SupplierService : ISupplierService
    {

        private readonly IRepository<Supplier , int> _supplierRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<SupplierService> _logger;

        public SupplierService(IRepository<Supplier, int> supplierRepository, IMapper mapper, ILogger<SupplierService> logger)
        {
            _supplierRepository = supplierRepository ?? throw new ArgumentNullException(nameof(supplierRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<GetSupplierDTO> CreateSupplierAsync(CreateSupplierDTO dto)
        {
            _logger.LogInformation("Attmpting to create supplier: {SupplierName}", dto.Name);
            try
            {
                if(dto is null)
                {
                    _logger.LogError("CreateSupplierAsync called with null DTO.");
                    throw new ArgumentNullException(nameof(dto),"Supplier DTO cannot be null");
                }
                var existingSupplier = await _supplierRepository.GetByPredicateAsync(s => dto.Name!.Equals(s.Name));
                if (existingSupplier is not null)
                {
                    _logger.LogWarning("Supplier with name {SupplierName} alreay exists.Creation FAILED.", dto.Name);
                    throw new InvalidOperationException($"Supplier with name '{dto.Name}' already exists.");

                }
                Supplier supplier = _mapper.Map<Supplier>(dto);
                await _supplierRepository.AddAsync(supplier);
                await _supplierRepository.SaveAsync();

                _logger.LogInformation("Supplier '{SupplierName}' (ID: {SupplierId}) created successfully.", supplier.Name, supplier.Id);
                return _mapper.Map<GetSupplierDTO>(supplier);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while adding supplier with Name: {SupplierName}", dto.Name);
                throw;
            }
        }

        public async Task DeleteSupplierAsync(int id)
        {
            _logger.LogInformation("Attempting to delete supplier with ID: {SupplierId}", id);
            try
            {
                var supplier = await _supplierRepository.GetByIdAsync(id);
                if (supplier is null)
                {
                    _logger.LogWarning("Supplier with ID: {SupplierId} not found for deletion.", id);
                    throw new KeyNotFoundException($"Supplier with ID '{id}' not found.");
                }
                if (supplier.Orders is not null && supplier.Orders.Count != 0)
                {
                    _logger.LogWarning("Can't delete Supplier with ID: {SupplierId} This Supplier is a foriegn key in Orders.", id);
                    throw new DbUpdateException($"Supplier with ID '{id}' is used by an order or more.");
                }
                _supplierRepository.Delete(supplier);
                await _supplierRepository.SaveAsync();

                _logger.LogInformation("Supplier with ID: {SupplierId} deleted successfully.", id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting supplier with ID {Id}.", id);
                throw;
            }

        }

        public async Task<IEnumerable<GetSupplierDTO>> GetAllSuppliersAsync()
        {
            _logger.LogInformation("Attempting to retreive all suppliers ");
            try
            {
                var suppliers =await _supplierRepository.GetAllAsync();
                _logger.LogInformation("Retreived all suppliers successfully");
                return _mapper.Map<IEnumerable<GetSupplierDTO>>(suppliers);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex,"Error occured while retreiving all suppliers");
                throw;
            }
        }

        public async Task<GetSupplierDTO> GetSupplierByIdAsync(int id)
        {
            _logger.LogInformation("Attempting to retrieve supplier with ID: {SupplierId}", id);
            try
            {
                var supplier = await _supplierRepository.GetByIdAsync(id);

                if (supplier is null)
                {
                    _logger.LogWarning("Supplier with ID: {SupplierId} not found.", id);
                    throw new KeyNotFoundException($"Supplier with ID '{id}' not found.");
                }

                _logger.LogInformation("Supplier with ID: {SupplierId} retrieved successfully.", id);
                return _mapper.Map<GetSupplierDTO>(supplier);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while retrieving supplier with id: {SupplierID}.", id);
                throw;
            }

        }

        public async Task UpdateSupplierAsync(UpdateSupplierDTO dto)
        {
            _logger.LogInformation("Attempting to update supplier with ID: {SupplierId}", dto.Id);
            if (dto is null)
            {
                _logger.LogWarning("UpdateSupplierAsync called with null DTO.");
                throw new ArgumentNullException(nameof(dto), "Supplier update DTO cannot be null.");
            }
            try
            {
                var supplier = await _supplierRepository.GetByIdAsync(dto.Id);
                if (supplier is null)
                {
                    _logger.LogWarning("Supplier with ID: {SupplierId} not found for update.", dto.Id);
                    throw new KeyNotFoundException($"Supplier with ID '{dto.Id}' not found.");
                }

                // if name is updated , we check for uniqueness
                if (!supplier.Name!.Equals(dto.Name, StringComparison.OrdinalIgnoreCase))
                {
                    var supplierWithSameName = await _supplierRepository.GetByPredicateAsync(c => c.Name!.Equals(dto.Name) && c.Id != dto.Id);
                    if (supplierWithSameName is not null)
                    {
                        _logger.LogWarning("Another supplier with name '{SupplierName}' already exists. Update failed for ID: {SupplierId}.", dto.Name, dto.Id);
                        throw new InvalidOperationException($"Another supplier with name '{dto.Name}' already exists.");
                    }
                }
                _mapper.Map(dto, supplier);
                _supplierRepository.Update(supplier);
                await _supplierRepository.SaveAsync();

                _logger.LogInformation("Supplier '{Name}' updated successfully.", supplier.Name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating supplier with ID {Id}.", dto.Id);
                throw;
            }
        }
    }
}
