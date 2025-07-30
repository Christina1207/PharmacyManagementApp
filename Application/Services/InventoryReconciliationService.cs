using Application.IServices.Inventory;
using Domain.Entities;
using Domain.IUnitOfWork;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class InventoryReconciliationService : IInventoryReconciliationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<InventoryReconciliationService> _logger;

        public InventoryReconciliationService(IUnitOfWork unitOfWork, ILogger<InventoryReconciliationService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task ReconcileInventoryAsync(int inventoryCheckId)
        {
            _logger.LogInformation("Starting reconciliation for Inventory Check ID: {InventoryCheckId}", inventoryCheckId);

            var inventoryCheck = await _unitOfWork.InventoryChecks.GetCheckWithDetailsAsync(inventoryCheckId);
            if (inventoryCheck == null)
            {
                throw new KeyNotFoundException($"Inventory Check with ID {inventoryCheckId} not found.");
            }

            foreach (var checkedItem in inventoryCheck.InventoryCheckItems)
            {
                var inventoryItem = await _unitOfWork.InventoryItems.GetByPredicateAsync(
                    i => i.MedicationId == checkedItem.MedicationId,
                    i => i.InventoryItemDetails);

                if (inventoryItem == null)
                {
                    _logger.LogWarning("Cannot reconcile Medication ID {MedicationId}: It does not exist in the main inventory.", checkedItem.MedicationId);
                    continue; // Skip this item if it's not in inventory
                }

                // Clear existing batches for this item
                var existingBatches = inventoryItem.InventoryItemDetails.ToList();
                foreach (var batch in existingBatches)
                {
                    _unitOfWork.InventoryItemDetails.Delete(batch);
                }

                // Create a single new batch with the counted quantity
                // A real-world app might require a form to specify expiry dates, but for now, we'll create one reconciled batch.
                if (checkedItem.CountedQuantity > 0)
                {
                    var newBatch = new InventoryItemDetail
                    {
                        ItemId = inventoryItem.Id,
                        Quantity = checkedItem.CountedQuantity,
                        // Using a far-future date as a placeholder for reconciled stock
                        ExpirationDate = new DateOnly(DateTime.Now.Year + 5, 1, 1)
                    };
                    await _unitOfWork.InventoryItemDetails.AddAsync(newBatch);
                }

                _logger.LogInformation("Reconciled Medication ID {MedicationId}. Old Qty: {Expected}, New Qty: {Counted}",
                    checkedItem.MedicationId, checkedItem.ExpectedQuantity, checkedItem.CountedQuantity);
            }

            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation("Reconciliation complete for Inventory Check ID: {InventoryCheckId}", inventoryCheckId);
        }
    }
}
