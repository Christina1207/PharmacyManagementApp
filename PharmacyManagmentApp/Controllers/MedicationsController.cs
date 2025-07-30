using Application.DTOs.Medication;
using Application.IServices.Medication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace PharmacyManagmentApp.Controllers
{

    [Authorize(Roles = "Admin,Pharmacist",AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    [Route("[controller]")]
    public class MedicationsController : Controller
    {
        private readonly IMedicationService _medicationService;

        public MedicationsController(IMedicationService medicationService)
        {
            _medicationService = medicationService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? search)
        {
            var medications = await _medicationService.GetAllMedicationsAsync();
            if (!string.IsNullOrEmpty(search))
            {
                var lowercasedSearch = search.ToLowerInvariant();
                medications = medications.Where(m =>
                    (m.Name != null && m.Name.ToLowerInvariant().Contains(lowercasedSearch)) ||
                    (m.Barcode != null && m.Barcode.ToLowerInvariant().Contains(lowercasedSearch))
                );
            }
            return Ok(medications);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var medication = await _medicationService.GetMedicationByIdAsync(id);
                return Ok(medication);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateMedicationDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                var newMedication = await _medicationService.CreateMedicationAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = newMedication.Id }, newMedication);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { Message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateMedicationDTO dto)
        {
            if (id != dto.Id) return BadRequest("ID in URL must match ID in request body.");
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                await _medicationService.UpdateMedicationAsync(dto);
                return NoContent(); // Standard response for a successful update
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _medicationService.DeleteMedicationAsync(id);
                return NoContent(); // Standard response for a successful delete
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
            catch (DbUpdateException ex)
            {
                // This catches the business rule violation from the service
                return Conflict(new { Message = ex.Message });
            }
        }
    }
}
