using Application.DTOs.InventoryCheck;
using Application.IServices.InventoryCheck;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace PharmacyManagmentApp.Controllers
{
    [Authorize(Roles = "Admin,Pharmacist", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    [Route("api/[controller]")]
    public class InventoryCheckController : ControllerBase
    {
        private readonly IInventoryCheckService _checkService;

        public InventoryCheckController(IInventoryCheckService checkService)
        {
            _checkService = checkService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateCheck([FromBody] CreateInventoryCheckDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userIdString)) return Unauthorized("User ID claim not found.");

                var userId = int.Parse(userIdString);
                var result = await _checkService.CreateInventoryCheckAsync(dto, userId);
                return CreatedAtAction(nameof(GetCheckById), new { id = result.Id }, result);
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCheckById(int id)
        {
            try
            {
                var check = await _checkService.GetCheckByIdAsync(id);
                return Ok(check);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllChecks()
        {
            var checks = await _checkService.GetAllChecksAsync();
            return Ok(checks);
        }
    }
}
