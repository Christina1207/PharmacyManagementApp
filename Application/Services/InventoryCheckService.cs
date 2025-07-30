using Application.DTOs.InventoryCheck;
using Application.IServices.InventoryCheck;
using AutoMapper;
using Domain.Entities;
using Domain.IUnitOfWork;
using Microsoft.Extensions.Logging;

namespace Application.Services
{
    public class InventoryCheckService : IInventoryCheckService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<InventoryCheckService> _logger;

        public InventoryCheckService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<InventoryCheckService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<GetInventoryCheckDTO> CreateInventoryCheckAsync(CreateInventoryCheckDTO dto, int userId)
        {
            _logger.LogInformation("Starting new inventory check for user {UserId}", userId);

            var inventoryCheck = new InventoryCheck
            {
                UserId = userId,
                Date = DateTime.UtcNow,
                Notes = dto.Notes
            };

            foreach (var itemDto in dto.Items)
            {
                var inventoryItem = await _unitOfWork.InventoryItems.GetByPredicateAsync(
                    i => i.MedicationId == itemDto.MedicationId,
                    i => i.InventoryItemDetails);

                int expectedQuantity = inventoryItem?.InventoryItemDetails.Sum(d => d.Quantity) ?? 0;

                inventoryCheck.InventoryCheckItems.Add(new InventoryCheckItem
                {
                    MedicationId = itemDto.MedicationId,
                    CountedQuantity = itemDto.CountedQuantity,
                    ExpectedQuantity = expectedQuantity
                });
            }

            await _unitOfWork.InventoryChecks.AddAsync(inventoryCheck);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Successfully created inventory check with ID {CheckId}", inventoryCheck.Id);
            return await GetCheckByIdAsync(inventoryCheck.Id);
        }

        public async Task<GetInventoryCheckDTO> GetCheckByIdAsync(int id)
        {
            // Use the new, clean repository method
            var check = await _unitOfWork.InventoryChecks.GetCheckWithDetailsAsync(id);

            if (check == null) throw new KeyNotFoundException($"Inventory Check with ID {id} not found.");

            return _mapper.Map<GetInventoryCheckDTO>(check);
        }
        public async Task<IEnumerable<GetInventoryCheckDTO>> GetAllChecksAsync()
        {
            // Use the new, clean repository method
            var checks = await _unitOfWork.InventoryChecks.GetAllChecksWithDetailsAsync();
            return _mapper.Map<IEnumerable<GetInventoryCheckDTO>>(checks);
        }
    }
}
