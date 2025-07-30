using Application.DTOs.Reporting;
using Application.IServices.Reporting;
using Domain.IUnitOfWork;
using Microsoft.Extensions.Logging;

namespace Application.Services
{
    public class ReportService : IReportService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ReportService> _logger;

        public ReportService(IUnitOfWork unitOfWork, ILogger<ReportService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<FinancialReportDTO> GetFinancialReportAsync(DateTime startDate, DateTime endDate)
        {
            _logger.LogInformation("Generating financial report from {StartDate} to {EndDate}", startDate, endDate);

            var salesInDateRange = await _unitOfWork.Sales
                .GetAllByPredicateAsync(s => s.Prescription.DispenseDate >= startDate && s.Prescription.DispenseDate <= endDate);

            var report = new FinancialReportDTO
            {
                StartDate = startDate,
                EndDate = endDate,
                TotalSales = salesInDateRange.Count(),
                TotalAmount = salesInDateRange.Sum(s => s.TotalAmount),
                TotalDiscount = salesInDateRange.Sum(s => s.Discount),
                TotalRevenue = salesInDateRange.Sum(s => s.AmountReceived)
            };

            return report;
        }

        public async Task<InventoryReportDTO> GetInventoryReportAsync()
        {
            _logger.LogInformation("Generating inventory report");

            var inventoryItems = await _unitOfWork.InventoryItems.GetAllAsync(
                i => i.Medication,
                i => i.InventoryItemDetails
            );

            var reportItems = inventoryItems.Select(item => new InventoryReportItemDTO
            {
                MedicationId = item.MedicationId,
                MedicationName = item.Medication.Name,
                TotalQuantity = item.InventoryItemDetails.Sum(d => d.Quantity),
                UnitPrice = item.Price,
                IsBelowMinStock = item.InventoryItemDetails.Sum(d => d.Quantity) < item.Medication.MinQuantity,
                DaysUntilEarliestExpiry = item.InventoryItemDetails
                                            .Where(d => d.Quantity > 0)
                                            .Select(d => (int?)d.ExpirationDate.ToDateTime(TimeOnly.MinValue).Subtract(DateTime.UtcNow).TotalDays)
                                            .OrderBy(d => d)
                                            .FirstOrDefault() ?? -1
            }).ToList();

            var report = new InventoryReportDTO
            {
                GeneratedAt = DateTime.UtcNow,
                TotalItemsInStock = reportItems.Count,
                LowStockItemsCount = reportItems.Count(i => i.IsBelowMinStock),
                TotalInventoryValue = reportItems.Sum(i => i.TotalValue),
                Items = reportItems
            };

            return report;
        }
    }
}
