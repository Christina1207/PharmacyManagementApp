using Application.DTOs.Employee;
using Application.DTOs.FamilyMember;
using Application.IServices.Employee;
using Application.IServices.FamilyMember;
using Application.IServices.InsuredPerson;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PharmacyManagmentApp.Controllers
{
    [Authorize(Roles = "Admin", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    [Route("api/admin/patients")]
    public class PatientsController : Controller
    {
        private readonly IInsuredPersonService _insuredPersonService;
        private readonly IEmployeeService _employeeService;
        private readonly IFamilyMemberService _familyMemberService;

        public PatientsController(IInsuredPersonService insuredPersonService, IEmployeeService employeeService, IFamilyMemberService familyMemberService)
        {
            _insuredPersonService = insuredPersonService;
            _employeeService = employeeService;
            _familyMemberService = familyMemberService;
         
        }


        [HttpGet] // GET /api/admin/patients
        public async Task<IActionResult> GetAllPatients()
        {
            var patients = await _insuredPersonService.GetAllInsuredPersonsAsync();
            return Ok(patients);
        }

        [HttpGet("{id}")]  // GET /api/admin/patients/{id}
        public async Task<IActionResult> GetPatientById(int id)
        {
            try
            {
                var patient = await _insuredPersonService.GetInsuredPersonByIdAsync(id);
                return Ok(patient);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
        }

        [HttpPut("{id}/activate")]  // PUT /api/admin/patients/{id}/activate
        public async Task<IActionResult> ActivatePatient(int id)
        {
            try
            {
                await _insuredPersonService.ActivateInsuredPersonAsync(id);
                return Ok(new { Message = $"Patient with ID {id} has been activated." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
        }

        [HttpPut("{id}/deactivate")] // PUT /api/admin/patients/{id}/deactivate
        public async Task<IActionResult> DeactivatePatient(int id)
        {
            try
            {
                await _insuredPersonService.DeactivateInsuredPersonAsync(id);
                return Ok(new { Message = $"Patient with ID {id} has been deactivated." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
        }



        /*  Employees  */

        [HttpPost("employees/{employeeId}/familymembers")] // POST /api/admin/patients/employees/{employeeId}/familymembers
        public async Task<IActionResult> AddFamilyMember(int employeeId, [FromBody] CreateFamilyMemberDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                var newFamilyMember = await _familyMemberService.AddFamilyMemberToEmployeeAsync(employeeId, dto);
                return Ok(newFamilyMember);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
        }

        [HttpPost("employees")] // POST /api/admin/patients/employees
        public async Task<IActionResult> CreateEmployee([FromBody] CreateEmployeeDTO dto)
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
                var newEmp = await _employeeService.CreateEmployeeAsync(dto);
                return CreatedAtAction(nameof(GetEmployeeById), new { id = newEmp.Id }, newEmp);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Error = "Failed to create employee",
                    Details = ex.Message
                });
            }

        }

        [HttpGet("employees/{id}")]// GET /api/admin/patients/employees/{id}
        public async Task<IActionResult> GetEmployeeById(int id)
        {
            try
            {
                var dep = await _employeeService.GetEmployeeByIdAsync(id);
                if (dep == null) return NotFound(new { Error = $"Employee with ID {id} not found" });
                return Ok(dep);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Error = $"An error occurred while retrieving employee with ID {id}",
                    Details = ex.Message
                });
            }
        }





    }
}
