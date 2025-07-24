using Application.DTOs.Inventory;
using Application.IServices.IInventoryService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PharmacyManagmentApp.Controllers
{
    [Authorize (Roles ="Admin,Pharmacist",AuthenticationSchemes =JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    [Route("[controller]")]
    public class InventoryController :ControllerBase
    {
        private readonly IInventoryService _inventoryService;

        public InventoryController(IInventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var inventory = await _inventoryService.GetAllInventoryItemsAsync();
            return Ok(inventory);
        }

        [HttpGet("medication/{medicationId}")]
        public async Task<IActionResult> GetByMedicationId(int medicationId)
        {
            try
            {
                var item = await _inventoryService.GetInventoryItemByMedicationIdAsync(medicationId);
                return Ok(item);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
        }

        [HttpPost("stock")]
        public async Task<IActionResult> AddStock([FromBody] AddStockDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var updatedItem = await _inventoryService.AddStockAsync(dto);
                return Ok(updatedItem);
            }
            catch (Exception ex)
            {
                // Generic error handler for unexpected issues
                return StatusCode(500, new { Message = "An error occurred while adding stock.", Details = ex.Message });
            }
        }

    }
}
