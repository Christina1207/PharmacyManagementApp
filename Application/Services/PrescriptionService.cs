using Application.DTOs.Prescription;
using Application.DTOs.Sale;
using Application.IServices.Prescription;
using AutoMapper;
using Domain.Entities;
using Domain.IUnitOfWork;
using Microsoft.Extensions.Logging;


namespace Application.Services
{
    public class PrescriptionService : IPrescriptionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<PrescriptionService> _logger;

        public PrescriptionService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<PrescriptionService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<GetSaleDTO> ProcessPrescriptionAsync(CreatePrescriptionDTO dto)
        {
            _logger.LogInformation("Starting to process prescription for Patient ID: {PatientId}", dto.PatientId);

            var prescription = _mapper.Map<Prescription>(dto);
            prescription.DispenseDate = DateTime.UtcNow;

            decimal totalValue = 0;

            foreach (var itemDto in dto.PrescriptionItems!)
            {
                var inventoryItem = await _unitOfWork.InventoryItems.GetByPredicateAsync(i => i.MedicationId == itemDto.MedicationId);
                if (inventoryItem == null)
                {
                    _logger.LogWarning("Medication ID: {MedicationId} is not stocked in inventory.", itemDto.MedicationId);
                    throw new InvalidOperationException($"Medication ID {itemDto.MedicationId} is not a stock item.");
                }
                var availableBatches = (await _unitOfWork.InventoryItemDetails
                    .GetAllByPredicateAsync(d => d.ItemId == inventoryItem.Id && d.Quantity > 0))
                    .OrderBy(d => d.ExpirationDate)
                    .ToList();

                var totalStock = availableBatches.Sum(b => b.Quantity);
                if (totalStock < itemDto.Quantity)
                {
                    _logger.LogWarning("Insufficient stock for Medication ID: {MedicationId}. Required: {Required}, Available: {Available}",
                        itemDto.MedicationId, itemDto.Quantity, totalStock);
                    throw new InvalidOperationException($"Insufficient stock for Medication ID {itemDto.MedicationId}. Only {totalStock} available.");
                }
                var quantityToDispense = itemDto.Quantity;
                foreach (var batch in availableBatches)
                {
                    if (quantityToDispense == 0) break;

                    var quantityFromThisBatch = Math.Min(batch.Quantity, quantityToDispense);

                    batch.Quantity -= quantityFromThisBatch;
                    quantityToDispense -= quantityFromThisBatch;

                    _unitOfWork.InventoryItemDetails.Update(batch);
                }

                var itemValue = inventoryItem.Price * itemDto.Quantity;
                totalValue += itemValue;

                prescription.PrescriptionItems.Add(new PrescriptionItem
                {
                    MedicationId = itemDto.MedicationId,
                    Quantity = itemDto.Quantity,
                    UnitPrice = inventoryItem.Price
                });
            }
        

            prescription.TotalValue = totalValue;
            await _unitOfWork.Prescriptions.AddAsync(prescription);

            prescription.TotalValue = totalValue;
            await _unitOfWork.Prescriptions.AddAsync(prescription);

            var sale = new Sale
            {
                Prescription = prescription,
                TotalAmount = totalValue,
                Discount = 0, // Add discount Logic
                AmountReceived = totalValue,
                UserId = dto.UserId
            };
            await _unitOfWork.Sales.AddAsync(sale);

            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Successfully processed prescription ID: {PrescriptionId} and created Sale ID: {SaleId}", prescription.Id, sale.Id);
            var finalSale = await _unitOfWork.Sales.GetByIdAsync(sale.Id);
            return _mapper.Map<GetSaleDTO>(finalSale);
        }
    }
}
