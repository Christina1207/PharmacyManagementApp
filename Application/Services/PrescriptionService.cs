using Application.DTOs.Prescription;
using Application.DTOs.Sale;
using Application.IServices.Prescription;
using Application.Utilities;
using AutoMapper;
using Domain.Entities;
using Domain.IUnitOfWork;
using Microsoft.Extensions.Logging;
using System.Formats.Tar;


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

            var patient = await _unitOfWork.InsuredPersons.GetByIdAsync(dto.PatientId);
            if(patient == null)
            {
                throw new KeyNotFoundException("$Patient with ID {dto.PatientId} not found");
            }

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


            decimal patientShare = PatientShareCalculator.Calculate(patient.Type, totalValue);

            var sale = new Sale
            {
                Prescription = prescription,
                TotalAmount = totalValue,
                Discount = totalValue - patientShare,
                AmountReceived = patientShare,
                UserId = dto.UserId
            };
            await _unitOfWork.Sales.AddAsync(sale);

            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Successfully processed prescription ID: {PrescriptionId} and created Sale ID: {SaleId}", prescription.Id, sale.Id);
            var finalSale = await _unitOfWork.Sales.GetByIdAsync(sale.Id, s=>s.User);
            return _mapper.Map<GetSaleDTO>(finalSale);
        }

        public async Task<GetPrescriptionDTO>GetPrescriptionByIdAsync(int id)
        {
            _logger.LogInformation("Retrieving prescription by ID: {Id}", id);
            var prescription = await _unitOfWork.Prescriptions.GetByIdAsync(id);
            if (prescription == null)
            {
                throw new KeyNotFoundException($"Prescription with ID {id} not found.");
            }
            return _mapper.Map<GetPrescriptionDTO>(prescription);
        }
        public async Task<IEnumerable<GetPrescriptionDTO>> GetAllPrescriptionsAsync()
        {
            _logger.LogInformation("Retrieving all prescriptions.");
            var prescriptions = await _unitOfWork.Prescriptions.GetAllAsync(p=>p.Patient, p=>p.Doctor, p=>p.User, p=>p.PrescriptionItems);
            return _mapper.Map<IEnumerable<GetPrescriptionDTO>>(prescriptions);
        }
    }
}
