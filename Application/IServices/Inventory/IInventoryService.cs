using Application.DTOs.Inventory;

namespace Application.IServices.IInventoryService
{
    public interface IInventoryService
    {
        public Task<GetInventoryItemDTO> AddStockAsync(AddStockDTO dto);
        public Task<IEnumerable<GetInventoryItemDTO>> GetAllInventoryItemsAsync();
        public Task<GetInventoryItemDTO> GetInventoryItemByMedicationIdAsync(int medicationId);
    }
}
