using Application.DTOs.Manufacturer;
using Application.IServices.Manufacturer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace PharmacyManagmentApp.Controllers
{
    [Authorize(Roles = "Pharmacist", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    [Route("api/manufacturers")]
    public class ManufacturersController : Controller
    {
        private readonly IManufacturerService _manufacturerService;
        public ManufacturersController(IManufacturerService manufacturerService)
        {
            _manufacturerService = manufacturerService;
        }


        [HttpGet]
        public async Task<IActionResult> GetManufacturers()
        {

            try
            {
                var result = await _manufacturerService.GetAllManufacturersAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Error = "Failed to retrieve manufacturers",
                    Details = ex.Message
                });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetManufacturerById(int id)
        {
            try
            {
                var sup = await _manufacturerService.GetManufacturerByIdAsync(id);
                if (sup == null) return NotFound(new { Error = $"Manufacturer with ID {id} not found" });
                return Ok(sup);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Error = $"An error occurred while retrieving manufacturer with ID {id}",
                    Details = ex.Message
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateManufacturer([FromBody] CreateManufacturerDTO dto)
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
                var newSup = await _manufacturerService.CreateManufacturerAsync(dto);
                return CreatedAtAction(nameof(GetManufacturerById), new { id = newSup.Id }, newSup);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Error = "Failed to create manufacturer",
                    Details = ex.Message
                });
            }

        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateManufacturer(int id, [FromBody] UpdateManufacturerDTO dto)
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
                await _manufacturerService.UpdateManufacturerAsync(dto);
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
                    Error = $"An error occurred while updating manufacturer with ID {id}",
                    Details = ex.Message
                });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteManufacturer(int id)
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
                await _manufacturerService.DeleteManufacturerAsync(id);
                return Ok($"Deleted manufacturer with id {id}");
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
                    Error = $"An error occurred while Deleting Manufacturer with ID {id}",
                    Details = ex.Message
                });
            }
        }


    }
}
