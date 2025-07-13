

using Application.DTOs.Supplier;
using Application.IServices.Supplier;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace PharmacyManagmentApp.Areas.Pharmacist
{
    [ApiController]
    [Route("[controller]")]
    public class PharmacistDashboardController : ControllerBase
    {
        private readonly ISupplierService _supplierService;
        public PharmacistDashboardController(ISupplierService supplierService)
        {
            _supplierService = supplierService;
        }
        [HttpGet] // No route parameter
        public IActionResult GetDefault()
        {
            return Ok(new { Message = "Welcome to the Pharmacist Dashboard" });

        }
        [HttpGet("Suppliers")]
        public async Task<IActionResult> GetSuppliers()
        {

            try
            {
                var result = await _supplierService.GetAllSuppliersAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Error = "Failed to retrieve suppliers",
                    Details = ex.Message
                });
            }
        }

        [HttpGet("Suppliers/{id}")]
        public async Task<IActionResult> GetSupplierById(int id)
        {
            try
            {
                var sup = await _supplierService.GetSupplierByIdAsync(id);
                if (sup == null) return NotFound(new { Error = $"Supplier with ID {id} not found" });
                return Ok(sup);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Error = $"An error occurred while retrieving supplier with ID {id}",
                    Details = ex.Message
                });
            }
        }

        [HttpPost("Suppliers")]
        public async Task<IActionResult> CreateSupplier([FromBody] CreateSupplierDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    Error = "Validation failed",
                    Details = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                });
            }
            try
            {
                var newSup = await _supplierService.CreateSupplierAsync(dto);
                return CreatedAtAction(nameof(GetSupplierById), new { id = newSup.Id }, newSup);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Error = "Failed to create supplier",
                    Details = ex.Message
                });
            }

        }

        [HttpPut("Suppliers/{id}")]
        public async Task<IActionResult> UpdateSupplier(int id, [FromBody] UpdateSupplierDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    Error = "Validation failed",
                    Details = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                });
            }
            try
            {
                await _supplierService.UpdateSupplierAsync(dto);
                return Ok(dto);
            }
            catch (KeyNotFoundException e)
            {
                return NotFound(new { Error = e.Message });

            }
            catch (DbUpdateException e)
            {
                return Conflict(new { Error = e.Message });

            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Error = $"An error occurred while updating supplier with ID {id}",
                    Details = ex.Message
                });
            }
        }

        [HttpDelete("Suppliers/{id}")]
        public async Task<IActionResult> DeleteSupplier(int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    Error = "Validation failed",
                    Details = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                });
            }
            try
            {
                await _supplierService.DeleteSupplierAsync(id);
                return Ok($"Deleted supplier with id {id}");
            }
            catch (KeyNotFoundException e)
            {
                return NotFound(new { Error = e.Message });

            }
            catch (DbUpdateException e)
            {
                return Conflict(new { Error = e.Message });

            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Error = $"An error occurred while Deleting Supplier with ID {id}",
                    Details = ex.Message
                });
            }
        }
    }
}
