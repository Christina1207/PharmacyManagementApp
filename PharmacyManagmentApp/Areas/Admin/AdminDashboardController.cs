using Application.DTOs.Department;
using Application.DTOs.Doctor;
using Application.IServices.Department;
using Application.IServices.Doctor;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.ComponentModel;
using Application.IServices.Employee;
using Application.IServices.FamilyMember;
using Application.IServices.InsuredPerson;
using Application.DTOs.Employee;
using Application.DTOs.FamilyMember;

namespace PharmacyManagmentApp.Areas.Admin
{
    [Area("Admin")]
    [Authorize(Roles ="Admin", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    [Route("api/[area]/[controller]")]

    public class AdminDashboardController : ControllerBase
    {
        private readonly IDepartmentService _departmentService;
        private readonly IDoctorService _doctorService;
        private readonly IEmployeeService _employeeService;
        private readonly IFamilyMemberService _familyMemberService;
        private readonly IInsuredPersonService _insuredPersonService;
        public AdminDashboardController(IDepartmentService departmentService,
            IDoctorService doctorService,
            IEmployeeService employeeService,
            IFamilyMemberService familyMemberService,
            IInsuredPersonService insuredPersonService)
        {
            _departmentService = departmentService;
            _doctorService = doctorService;
            _employeeService = employeeService;
            _familyMemberService = familyMemberService;
            _insuredPersonService = insuredPersonService;
        }
        [HttpGet] // No route parameter
        public IActionResult GetDefault()
        {
            return Ok(new { Message = "Welcome to the Admin Dashboard" });

        }

        // * Departments*  //
        [HttpGet("Departments")]
        public async Task<IActionResult> GetDepartments()
        {

            try
            {
                var result = await _departmentService.GetAllDepartmentsAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Error = "Failed to retrieve departments",
                    Details = ex.Message
                });
            }
        }

        [HttpGet("Departments/{id}")]
        public async Task<IActionResult> GetDepartmentById(int id)
        {
            try
            {
                var dep = await _departmentService.GetDepartmentByIdAsync(id);
                if (dep == null) return NotFound(new { Error = $"Department with ID {id} not found" });
                return Ok(dep);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Error = $"An error occurred while retrieving department with ID {id}",
                    Details = ex.Message
                });
            }
        }

        [HttpPost("Departments")]
        public async Task<IActionResult> CreateDepartment([FromBody] CreateDepartmentDTO dto)
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
                var newDep = await _departmentService.CreateDepartmentAsync(dto);
                return CreatedAtAction(nameof(GetDepartmentById), new { id = newDep.Id }, newDep);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Error = "Failed to create department",
                    Details = ex.Message
                });
            }

        }

        [HttpPut("Departments/{id}")]
        public async Task<IActionResult> UpdateDepartment(int id, [FromBody] UpdateDepartmentDTO dto)
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
                await _departmentService.UpdateDepartmentAsync(dto);
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
                    Error = $"An error occurred while updating department with ID {id}",
                    Details = ex.Message
                });
            }
        }

        [HttpDelete("Departments/{id}")]
        public async Task<IActionResult> DeleteDepartment(int id)
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
                await _departmentService.DeleteDepartmentAsync(id);
                return Ok($"Deleted department with id {id}");
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
                    Error = $"An error occurred while Deleting Department with ID {id}",
                    Details = ex.Message
                });
            }
        }

        // * Doctors * //

        [HttpGet("Doctors")]
        public async Task<IActionResult> GetDoctors()
        {

            try
            {
                var result = await _doctorService.GetAllDoctorsAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Error = "Failed to retrieve doctors",
                    Details = ex.Message
                });
            }
        }

        [HttpGet("Doctors/{id}")]
        public async Task<IActionResult> GetDoctorById(int id)
        {
            try
            {
                var dep = await _doctorService.GetDoctorByIdAsync(id);
                if (dep == null) return NotFound(new { Error = $"Doctor with ID {id} not found" });
                return Ok(dep);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Error = $"An error occurred while retrieving doctor with ID {id}",
                    Details = ex.Message
                });
            }
        }

        [HttpPost("Doctor")]
        public async Task<IActionResult> CreateDoctor([FromBody] CreateDoctorDTO dto)
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
                var newDep = await _doctorService.CreateDoctorAsync(dto);
                return CreatedAtAction(nameof(GetDoctorById), new { id = newDep.Id }, newDep);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Error = "Failed to create doctor",
                    Details = ex.Message
                });
            }

        }

        [HttpPut("Doctors/{id}")]
        public async Task<IActionResult> UpdateDoctor(int id, [FromBody] UpdateDoctorDTO dto)
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
                await _doctorService.UpdateDoctorAsync(dto);
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
                    Error = $"An error occurred while updating doctor with ID {id}",
                    Details = ex.Message
                });
            }
        }

        [HttpDelete("Doctors/{id}")]
        public async Task<IActionResult> DeleteDoctor(int id)
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
                await _doctorService.DeleteDoctorAsync(id);
                return Ok($"Deleted doctor with id {id}");
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
                    Error = $"An error occurred while Deleting Doctor with ID {id}",
                    Details = ex.Message
                });
            }
        }


        // * Employee * //

        [HttpGet("Employees")]
        public async Task<IActionResult> GetEmployees()
        {

            try
            {
                var result = await _employeeService.GetAllEmployeesAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Error = "Failed to retrieve employees",
                    Details = ex.Message
                });
            }
        }


        [HttpGet("Employees/{id}")]
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


        [HttpPost("Employee")]
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


        [HttpDelete("Employees/{id}")]
        public async Task<IActionResult> DeleteEmployee(int id)
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
                await _employeeService.DeleteEmployeeAsync(id);
                return Ok($"Deleted employee with id {id}");
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
                    Error = $"An error occurred while Deleting Employee with ID {id}",
                    Details = ex.Message
                });
            }
        }


        // * Family Member * //
        [HttpPost("Employees/{employeeId}/familymembers")]
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

        [HttpGet("Employees/{employeeId}/familymembers")]
        public async Task<IActionResult> GetFamilyMembers(int employeeId)
        {
            try
            {
                var familyMembers = await _familyMemberService.GetEmployeeFamilyMembersAsync(employeeId);
                return Ok(familyMembers);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
        }


        // * Insured Person * //

        [HttpGet("Patients")]
        public async Task<IActionResult> GetAllPatients()
        {
            var patients = await _insuredPersonService.GetAllInsuredPersonsAsync();
            return Ok(patients);
        }

        [HttpGet("Patients/{id}")]
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

        [HttpPut("Patient/{id}/activate")]
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


        [HttpPut("Patient/{id}/deactivate")]
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

    }
}
