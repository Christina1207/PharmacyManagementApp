using Application.DTOs.Prescription;
using Application.IServices.Prescription;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace PharmacyManagmentApp.Controllers
{
    [Authorize(Roles = "Pharmacist,Admin", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    [Route("[controller]")]
    public class PrescriptionController : ControllerBase
    {
        private readonly IPrescriptionService _prescriptionService;
        private readonly ILogger<PrescriptionController> _logger; 

        public PrescriptionController(IPrescriptionService prescriptionService,ILogger<PrescriptionController> logger)
        {
            _prescriptionService = prescriptionService;
            _logger = logger;
        }

        [HttpPost("dispense")]
        public async Task<IActionResult> Dispense([FromBody] CreatePrescriptionDTO dto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for Dispense request.");
                return BadRequest(ModelState);
            }
            try
            {
                var saleResult = await _prescriptionService.ProcessPrescriptionAsync(dto);
                return Ok(saleResult);
            }
            catch (InvalidOperationException ex)
            {
                // Catches business logic errors like "insufficient stock"
                _logger.LogWarning(ex, "Business logic error while processing prescription for Patient ID: {PatientId}", dto.PatientId);
                return Conflict(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An unexpected error occurred while processing the prescription.", Details = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPrescriptions()
        {
            var prescriptions = await _prescriptionService.GetAllPrescriptionsAsync();
            return Ok(prescriptions);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPrescriptionById(int id)
        {
            try
            {
                var prescription = await _prescriptionService.GetPrescriptionByIdAsync(id);
                return Ok(prescription);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
        }
    }
}
