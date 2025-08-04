using Application.IServices.Doctor;
using Application.IServices.InsuredPerson;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PharmacyManagmentApp.Controllers
{
    [Authorize(Roles = "Admin,Pharmacist", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    [Route("api/search")]
    public class SearchController : Controller
    {
        private readonly IInsuredPersonService _patientService;
        private readonly IDoctorService _doctorService;

        public SearchController(IInsuredPersonService patientService, IDoctorService doctorService)
        {
            _patientService = patientService;
            _doctorService = doctorService;
        }

        [HttpGet("patients")] // GET /api/search/patients?term=...
        public async Task<IActionResult> SearchPatients([FromQuery] string term)
        {
            var allPatients = await _patientService.GetAllInsuredPersonsAsync();
            var filtered = allPatients.Where(p =>
                p.FirstName.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                p.LastName.Contains(term, StringComparison.OrdinalIgnoreCase));
            return Ok(filtered);
        }

        [HttpGet("doctors")] // GET /api/search/doctors?term=...
        public async Task<IActionResult> SearchDoctors([FromQuery] string term)
        {
            var allDoctors = await _doctorService.GetAllDoctorsAsync();
            var filtered = allDoctors.Where(d =>
                d.FirstName.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                d.LastName.Contains(term, StringComparison.OrdinalIgnoreCase));
            return Ok(filtered);
        }
    }
}
