using Application.IServices.Reporting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PharmacyManagmentApp.Controllers
{
    [Authorize(Roles = "Admin",AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    [Route("api/[controller]")]
    public class ReportsController : ControllerBase
    {
        private readonly IReportService _reportService;

        public ReportsController(IReportService reportService)
        {
            _reportService = reportService;
        }

        [HttpGet("financial")]
        public async Task<IActionResult> GetFinancialReport([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            if (startDate > endDate)
            {
                return BadRequest("Start date cannot be after end date.");
            }
            var report = await _reportService.GetFinancialReportAsync(startDate, endDate);
            return Ok(report);
        }

        [HttpGet("inventory")]
        public async Task<IActionResult> GetInventoryReport()
        {
            var report = await _reportService.GetInventoryReportAsync();
            return Ok(report);
        }
    }
}
