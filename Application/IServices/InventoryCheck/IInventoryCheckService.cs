using Application.DTOs.InventoryCheck;

namespace Application.IServices.InventoryCheck
{   
     public interface IInventoryCheckService
     {
            Task<GetInventoryCheckDTO> CreateInventoryCheckAsync(CreateInventoryCheckDTO dto, int userId);
            Task<GetInventoryCheckDTO> GetCheckByIdAsync(int id);
            Task<IEnumerable<GetInventoryCheckDTO>> GetAllChecksAsync();
     }
}
