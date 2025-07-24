using Application.DTOs.Inventory;
using Application.IServices.IInventoryService;
using AutoMapper;
using Domain.Entities;
using Domain.IRepositories;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class InventoryService:IInventoryService
    {
        private readonly IRepository<InventoryItem, int> _inventoryItemRepository;
        private readonly IRepository<InventoryItemDetail, int> _inventoryDetailRepository;
        private readonly ILogger<InventoryService> _logger;
        private readonly IMapper _mapper;

        public InventoryService (
            IRepository<InventoryItem, int> inventoryItemRepository, 
            IRepository<InventoryItemDetail, int> inventoryDetailRepository,
            ILogger<InventoryService> logger, IMapper mapper)
        {
            _inventoryItemRepository = inventoryItemRepository;
            _inventoryDetailRepository = inventoryDetailRepository;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<GetInventoryItemDTO> AddStockAsync(AddStockDTO dto)
        {
            _logger.LogInformation("Attempting to add stock for Medication ID: {MedicationId}", dto.MedicationId);

            // look for the item in the inventory
            var inventoryItem = await _inventoryItemRepository.GetByPredicateAsync(i => i.MedicationId == dto.MedicationId);
            if (inventoryItem == null)
            {
                // if not found create a new inventory item
                _logger.LogInformation("No existing inventory item found for Medication ID {MedicationId}. Creating a new one.", dto.MedicationId);
                inventoryItem = new InventoryItem
                {
                    MedicationId = dto.MedicationId,
                    Price = dto.Price
                };
                await _inventoryItemRepository.AddAsync(inventoryItem);
                await _inventoryItemRepository.SaveAsync(); // Save to get the new ID
            }
            else
            {
                // Business Rule: Update price on new stock arrival if it has changed.
                if (inventoryItem.Price != dto.Price)
                {
                    _logger.LogInformation("Updating price for inventory item ID {InventoryItemId} from {OldPrice} to {NewPrice}", inventoryItem.Id, inventoryItem.Price, dto.Price);
                    inventoryItem.Price = dto.Price;
                    _inventoryItemRepository.Update(inventoryItem);
                }
            }

            // Find or create the batch (InventoryItemDetail)
            var batch = await _inventoryDetailRepository.GetByPredicateAsync(d => d.ItemId == inventoryItem.Id && d.ExpirationDate == dto.ExpirationDate);
            if (batch != null)
            {
                _logger.LogInformation("Existing batch found for Item ID {ItemId} with expiration {ExpirationDate}. Increasing quantity.", inventoryItem.Id, dto.ExpirationDate);
                batch.Quantity += dto.Quantity;
                _inventoryDetailRepository.Update(batch);
            }
            else
            {
                _logger.LogInformation("Creating new batch for Item ID {ItemId} with expiration {ExpirationDate}.", inventoryItem.Id, dto.ExpirationDate);
                batch = new InventoryItemDetail
                {
                    ItemId = inventoryItem.Id,
                    Quantity = dto.Quantity,
                    ExpirationDate = dto.ExpirationDate
                };
                await _inventoryDetailRepository.AddAsync(batch);
            }

            await _inventoryDetailRepository.SaveAsync();
            _logger.LogInformation("Stock added successfully for Medication ID: {MedicationId}", dto.MedicationId);

            return await GetInventoryItemByMedicationIdAsync(dto.MedicationId);
        }

        public async Task<IEnumerable<GetInventoryItemDTO>> GetAllInventoryItemsAsync()
        {
            _logger.LogInformation("Retrieving all inventory items.");
            var items = await _inventoryItemRepository.GetAllAsync(i=>i.Medication,i=>i.InventoryItemDetails);
            return _mapper.Map<IEnumerable<GetInventoryItemDTO>>(items);
        }

        public async Task<GetInventoryItemDTO> GetInventoryItemByMedicationIdAsync(int medicationId)
        {
            _logger.LogInformation("Retrieving inventory for Medication ID: {MedicationId}", medicationId);
            var item = await _inventoryItemRepository.GetByPredicateAsync(i => i.MedicationId == medicationId,i=>i.Medication,i => i.InventoryItemDetails);
            if (item == null)
            {
                _logger.LogWarning("Inventory item not found for Medication ID: {MedicationId}", medicationId);
                throw new KeyNotFoundException($"No inventory found for Medication ID {medicationId}.");
            }
            return _mapper.Map<GetInventoryItemDTO>(item);
        }
        

    }
}
