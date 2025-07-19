using Application.DTOs.InventoryItemDetail;

namespace Application.IServices.InventoryItemDetail
{
    public interface IInventoryItemDetailService
    {
        public Task<GetInventoryItemDetailDTO> CreateInventoryItemDetailAsync(CreateInventoryItemDetailDTO dto);

        public Task DeleteInventoryItemDetailAsync(int id);
        
        Task<GetInventoryItemDetailDTO> GetInventoryItemDetailByIdAsync(int id);

        Task AddQuantityToExistingBatchAsync(int inventoryItemDetailId, int quantityToAdd);

        // Get all details (expiration dates and quantities for batches )for a specific InventoryItem
        Task<IEnumerable<GetInventoryItemDetailDTO>> GetInventoryItemDetailsByItemIdAsync(int itemId);

        // Dispense quantity from an existing batch (identified by InventoryItemDetail ID) 
        Task DispenseFromBatchAsync(int inventoryItemDetailId, int quantityToDispense);


    }
}
