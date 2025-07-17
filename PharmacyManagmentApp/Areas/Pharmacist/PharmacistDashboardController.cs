

using Application.DTOs.ActiveIngredient;
using Application.DTOs.Diagnosis;
using Application.DTOs.Supplier;
using Application.IServices.ActiveIngredient;
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
        private readonly IActiveIngredientService _activeIngredientService;
        public PharmacistDashboardController(ISupplierService supplierService, IDiagnosisService diagnosisService, IActiveIngredientService activeIngredientService)
        {
            _supplierService = supplierService;
            _diagnosisService = diagnosisService;
            _activeIngredientService = activeIngredientService;
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

        [HttpGet("Diagnosis/{id}")]
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

        [HttpPost("Diagnosis")]
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

        [HttpPut("Diagnosis/{id}")]
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

        [HttpDelete("Diagnosis/{id}")]
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



        //* Active Ingredients *//

        [HttpGet("ActiveIngredients")]
        public async Task<IActionResult> GetActiveIngredient()
        {
            try
            {
                var result = await _activeIngredientService.GetAllActiveIngredientsAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Error = "Failed to retrieve Active Ingredients",
                    Details = ex.Message
                });
            }
        }

        [HttpGet("ActiveIngredient/{id}")]
        public async Task<IActionResult> GetActiveIngredientById(int id)
        {
            try
            {
                var ing  = await _activeIngredientService.GetActiveIngredientByIdAsync(id);
                if (ing == null) return NotFound(new { Error = $"Active Ingredient with ID {id} not found" });
                return Ok(ing);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Error = $"An error occurred while retrieving active ingredient with ID {id}",
                    Details = ex.Message
                });
            }
        }

        [HttpPost("ActiveIngredient")]
        public async Task<IActionResult> CreateActiveIngredient([FromBody] CreateActiveIngredientDTO dto)
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
                var newIng = await _activeIngredientService.CreateActiveIngredientAsync(dto);
                return CreatedAtAction(nameof(GetActiveIngredientById), new { id = newIng.Id }, newIng);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Error = "Failed to create active ingredient",
                    Details = ex.Message
                });
            }

        }

        [HttpPut("ActiveIngredient/{id}")]
        public async Task<IActionResult> UpdateActiveIngredient(int id, [FromBody] UpdateActiveIngredientDTO dto)
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
                await _activeIngredientService.UpdateActiveIngredientAsync(dto);
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
                    Error = $"An error occurred while updating active ingredient with ID {id}",
                    Details = ex.Message
                });
            }
        }

        [HttpDelete("ActiveIngredient/{id}")]
        public async Task<IActionResult> DeleteActiveIngredient(int id)
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
                await _activeIngredientService.DeleteActiveIngredientAsync(id);
                return Ok($"Deleted active ingredient with id {id}");
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
                    Error = $"An error occurred while Deleting Active Ingredient with ID {id}",
                    Details = ex.Message
                });
            }
        }







    }
}
