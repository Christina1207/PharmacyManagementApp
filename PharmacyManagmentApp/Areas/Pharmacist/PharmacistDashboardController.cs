

using Application.DTOs.Diagnosis;
using Application.DTOs.Supplier;
using Application.IServices.Diagnosis;
using Application.IServices.Supplier;
using Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace PharmacyManagmentApp.Areas.Pharmacist
{
    [ApiController]
    [Route("[controller]")]
    public class PharmacistDashboardController : ControllerBase
    {
        private readonly ISupplierService _supplierService;
        private readonly IDiagnosisService _diagnosisService;
        public PharmacistDashboardController(ISupplierService supplierService, IDiagnosisService diagnosisService)
        {
            _supplierService = supplierService;
            _diagnosisService = diagnosisService;
        }
        
        [HttpGet] // No route parameter
        public IActionResult GetDefault()
        {
            return Ok(new { Message = "Welcome to the Pharmacist Dashboard" });

        }

        //* Supplier *//
        
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


        //* Diagnosis *//

        [HttpGet("Diagnoses")]
        public async Task<IActionResult> GetDiagnoses()
        {
            try
            {
                var result = await _diagnosisService.GetAllDiagnosesAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Error = "Failed to retrieve Diagnoses",
                    Details = ex.Message
                });
            }
        }

        [HttpGet("Diagnoses/{id}")]
        public async Task<IActionResult> GetDiagnosisById(int id)
        {
            try
            {
                var diag = await _diagnosisService.GetDiagnosisByIdAsync(id);
                if (diag == null) return NotFound(new { Error = $"Diagnosis with ID {id} not found" });
                return Ok(diag);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Error = $"An error occurred while retrieving diagnosis with ID {id}",
                    Details = ex.Message
                });
            }
        }

        [HttpPost("Diagnosiss")]
        public async Task<IActionResult> CreateDiagnosis([FromBody] CreateDiagnosisDTO dto)
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
                var newDiag = await _diagnosisService.CreateDiagnosisAsync(dto);
                return CreatedAtAction(nameof(GetDiagnosisById), new { id = newDiag.Id }, newDiag);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Error = "Failed to create diagnosis",
                    Details = ex.Message
                });
            }

        }

        [HttpPut("Diagnosiss/{id}")]
        public async Task<IActionResult> UpdateDiagnosis(int id, [FromBody] UpdateDiagnosisDTO dto)
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
                await _diagnosisService.UpdateDiagnosisAsync(dto);
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
                    Error = $"An error occurred while updating diagnosis with ID {id}",
                    Details = ex.Message
                });
            }
        }

        [HttpDelete("Diagnosiss/{id}")]
        public async Task<IActionResult> DeleteDiagnosis(int id)
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
                await _diagnosisService.DeleteDiagnosisAsync(id);
                return Ok($"Deleted diagnosis with id {id}");
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
                    Error = $"An error occurred while Deleting Diagnosis with ID {id}",
                    Details = ex.Message
                });
            }
        }






    }
}
