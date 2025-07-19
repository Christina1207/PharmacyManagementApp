using Application.DTOs.InventoryItemDetail;
using Application.IServices.InventoryItemDetail;
using AutoMapper;
using Domain.Entities;
using Domain.IRepositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Services
{
    public class InventoryItemDetailService : IInventoryItemDetailService
    {
        private readonly IRepository<InventoryItemDetail, int> _inventoryItemDetailRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<InventoryItemDetailService> _logger;

        public InventoryItemDetailService(IRepository<InventoryItemDetail, int> inventoryItemDetailRepository, IMapper mapper, ILogger<InventoryItemDetailService> logger)
        {
            _inventoryItemDetailRepository = inventoryItemDetailRepository ?? throw new ArgumentNullException(nameof(inventoryItemDetailRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<GetInventoryItemDetailDTO> CreateInventoryItemDetailAsync(CreateInventoryItemDetailDTO dto)
        {
            _logger.LogInformation("Attempting to create inventory item detail (new batch) for Item ID: {ItemId}, Expiration Date: {ExpDate}, Quantity: {Qty}.", dto.ItemId, dto.ExpirationDate, dto.Quantity);
            try
            {
                if (dto is null)
                {
                    _logger.LogError("CreateInventoryItemDetailAsync called with null DTO.");
                    throw new ArgumentNullException(nameof(dto), "Inventory Item Detail DTO cannot be null.");
                }          

                // 2. Business Logic: Check if a batch with the exact same expiration date already exists for this item.
                // If it does, this should be an update to quantity, not a new batch creation.
                var existingDetailForSameBatch = await _inventoryItemDetailRepository.GetByPredicateAsync(
                    iid => iid.ItemId == dto.ItemId && iid.ExpirationDate == dto.ExpirationDate
                );

                if (existingDetailForSameBatch != null)
                {
                    _logger.LogWarning(
                        "Batch for Item ID {ItemId} with Expiration Date {ExpDate} already exists (ID: {ExistingDetailId}). Use 'AddQuantityToExistingBatchAsync' to add more stock.",
                        dto.ItemId, dto.ExpirationDate, existingDetailForSameBatch.Id
                    );
                    throw new InvalidOperationException(
                        $"A batch for item '{dto.ItemId}' with expiration date '{dto.ExpirationDate}' already exists. Please use the 'Add Quantity' function to add more stock to this batch."
                    );
                }

                // 3. Map DTO to entity and add
                var inventoryItemDetail = _mapper.Map<InventoryItemDetail>(dto);
                await _inventoryItemDetailRepository.AddAsync(inventoryItemDetail);
                await _inventoryItemDetailRepository.SaveAsync();

                _logger.LogInformation(
                    "Inventory item detail (ID: {Id}, Qty: {Qty}, Exp: {ExpDate}) created successfully for Item ID: {ItemId}.",
                    inventoryItemDetail.Id, inventoryItemDetail.Quantity, inventoryItemDetail.ExpirationDate, inventoryItemDetail.ItemId
                );
                return _mapper.Map<GetInventoryItemDetailDTO>(inventoryItemDetail);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while creating inventory item detail for Item ID: {ItemId}, Expiration Date: {ExpDate}.", dto.ItemId, dto.ExpirationDate);
                throw; // Re-throw the exception after logging
            }
        }


        public async Task AddQuantityToExistingBatchAsync(int inventoryItemDetailId, int quantityToAdd)
        {
            _logger.LogInformation("Attempting to add {QtyToAdd} to batch ID: {BatchId}.", quantityToAdd, inventoryItemDetailId);
            try
            {
                if (quantityToAdd <= 0)
                {
                    _logger.LogError("AddQuantityToExistingBatchAsync called with non-positive quantity: {QtyToAdd}.", quantityToAdd);
                    throw new ArgumentOutOfRangeException(nameof(quantityToAdd), "Quantity to add must be positive.");
                }

                var batch = await _inventoryItemDetailRepository.GetByIdAsync(inventoryItemDetailId);
                if (batch is null)
                {
                    _logger.LogWarning("Batch with ID: {BatchId} not found for adding quantity.", inventoryItemDetailId);
                    throw new KeyNotFoundException($"Batch with ID '{inventoryItemDetailId}' not found.");
                }

                batch.Quantity += quantityToAdd;
                _inventoryItemDetailRepository.Update(batch);
                await _inventoryItemDetailRepository.SaveAsync();

                _logger.LogInformation("Added {QtyToAdd} to batch ID: {BatchId}. New quantity: {NewQty}.", quantityToAdd, inventoryItemDetailId, batch.Quantity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding quantity to batch ID {BatchId}.", inventoryItemDetailId);
                throw;
            }
        }

        public async Task DeleteInventoryItemDetailAsync(int id)
        {
            _logger.LogInformation("Attempting to delete inventory item detail (batch) with ID: {Id}.", id);
            try
            {
                var inventoryItemDetail = await _inventoryItemDetailRepository.GetByIdAsync(id);
                if (inventoryItemDetail is null)
                {
                    _logger.LogWarning("Inventory item detail with ID: {Id} not found for deletion.", id);
                    throw new KeyNotFoundException($"Inventory item detail with ID '{id}' not found.");
                }

                // Business Logic: Only allow removal if quantity is zero
                if (inventoryItemDetail.Quantity > 0)
                {
                    _logger.LogWarning("Cannot delete batch ID: {Id} because its quantity is {Qty}. Only empty batches can be removed.", id, inventoryItemDetail.Quantity);
                    throw new InvalidOperationException($"Cannot delete batch '{id}' because its quantity is {inventoryItemDetail.Quantity}. Only empty batches can be removed.");
                }
                // Optional: Add a check for expiration date if you only remove expired empty batches.
                // if (inventoryItemDetail.ExpirationDate > DateOnly.FromDateTime(DateTime.Today))
                // {
                //     _logger.LogWarning("Cannot delete batch ID: {Id} because it is not expired and has quantity 0. Only expired empty batches can be removed.", id);
                //     throw new InvalidOperationException($"Cannot delete batch '{id}' because it is not expired and has quantity 0. Only expired empty batches can be removed.");
                // }

                _inventoryItemDetailRepository.Delete(inventoryItemDetail);
                await _inventoryItemDetailRepository.SaveAsync();

                _logger.LogInformation("Inventory item detail (batch) with ID: {Id} deleted successfully.", id);
            }
            catch (DbUpdateException ex) // Catch DbUpdateException for potential FK violations if other entities reference this detail
            {
                _logger.LogError(ex, "Cannot delete Inventory Item Detail with ID {Id}. It might be referenced by other entities or have active constraints.", id);
                throw new InvalidOperationException($"Cannot delete Inventory Item Detail with ID '{id}'. It might be referenced by other entities or have active constraints.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting inventory item detail with ID {Id}.", id);
                throw;
            }

        }

        public async Task DispenseFromBatchAsync(int inventoryItemDetailId, int quantityToDispense)
        {
            _logger.LogInformation("Attempting to dispense {QtyToDispense} from batch ID: {BatchId}.", quantityToDispense, inventoryItemDetailId);
            try
            {
                if (quantityToDispense <= 0)
                {
                    _logger.LogError("DispenseFromBatchAsync called with non-positive quantity: {QtyToDispense}.", quantityToDispense);
                    throw new ArgumentOutOfRangeException(nameof(quantityToDispense), "Quantity to dispense must be positive.");
                }

                var batch = await _inventoryItemDetailRepository.GetByIdAsync(inventoryItemDetailId);
                if (batch is null)
                {
                    _logger.LogWarning("Batch with ID: {BatchId} not found for dispensing.", inventoryItemDetailId);
                    throw new KeyNotFoundException($"Batch with ID '{inventoryItemDetailId}' not found.");
                }

                if (batch.Quantity < quantityToDispense)
                {
                    _logger.LogWarning("Insufficient stock in batch ID: {BatchId}. Available: {AvailableQty}, Requested: {RequestedQty}.", inventoryItemDetailId, batch.Quantity, quantityToDispense);
                    throw new InvalidOperationException($"Insufficient stock in batch '{inventoryItemDetailId}'. Available: {batch.Quantity}, Requested: {quantityToDispense}.");
                }

                batch.Quantity -= quantityToDispense;
                _inventoryItemDetailRepository.Update(batch);
                await _inventoryItemDetailRepository.SaveAsync();

                _logger.LogInformation("Dispensed {QtyToDispense} from batch ID: {BatchId}. Remaining quantity: {RemainingQty}.", quantityToDispense, inventoryItemDetailId, batch.Quantity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while dispensing from batch ID {BatchId}.", inventoryItemDetailId);
                throw;
            }
        }

        public async Task<GetInventoryItemDetailDTO> GetInventoryItemDetailByIdAsync(int id)
        {
            _logger.LogInformation("Attempting to retrieve inventory item detail with ID: {Id}.", id);
            try
            {
                var inventoryItemDetail = await _inventoryItemDetailRepository.GetByIdAsync(id);

                if (inventoryItemDetail is null)
                {
                    _logger.LogWarning("Inventory item detail with ID: {Id} not found.", id);
                    throw new KeyNotFoundException($"Inventory item detail with ID '{id}' not found.");
                }

                _logger.LogInformation("Inventory item detail with ID: {Id} retrieved successfully.", id);
                return _mapper.Map<GetInventoryItemDetailDTO>(inventoryItemDetail);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while retrieving inventory item detail with id: {Id}.", id);
                throw;
            }

        }

        public async Task<IEnumerable<GetInventoryItemDetailDTO>> GetInventoryItemDetailsByItemIdAsync(int itemId)
        {

            _logger.LogInformation("Attempting to retrieve all inventory item details for Item ID: {ItemId}.", itemId);
            try
            {
                // Optional: Validate if InventoryItem itself exists before querying its details
                // var existingItem = await _inventoryItemRepository.GetByIdAsync(itemId);
                // if (existingItem == null)
                // {
                //     _logger.LogWarning("InventoryItem with ID {ItemId} not found. Cannot retrieve details.", itemId);
                //     throw new KeyNotFoundException($"InventoryItem with ID '{itemId}' not found. Cannot retrieve details.");
                // }

                var itemDetails = await _inventoryItemDetailRepository.GetAllByPredicateAsync(detail => detail.ItemId == itemId);
                _logger.LogInformation("Retrieved {Count} inventory item details for Item ID: {ItemId} successfully.", itemDetails.Count(), itemId);
                return _mapper.Map<IEnumerable<GetInventoryItemDetailDTO>>(itemDetails);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving inventory item details for Item ID: {ItemId}.", itemId);
                throw;
            }
        }
    }
}
