using Application.DTOs.Department;
using Application.IServices.Department;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace PharmacyManagmentApp.Areas.Admin
{
    [ApiController]
    [Route("[controller]")]

    public class AdminDashboardController : ControllerBase
    {
        private readonly IDepartmentService _departmentService;
        public AdminDashboardController(IDepartmentService departmentService)
        {
            _departmentService = departmentService;
        }
        [HttpGet] // No route parameter
        public IActionResult GetDefault()
        {
            return Ok(new { Message = "Welcome to the Admin Dashboard" });

        }
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
    }
}
