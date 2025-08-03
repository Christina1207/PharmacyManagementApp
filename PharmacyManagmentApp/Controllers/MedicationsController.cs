using Application.DTOs.ActiveIngredient;
using Application.DTOs.Medication;
using Application.DTOs.MedicationClass;
using Application.DTOs.MedicationForm;
using Application.IServices.ActiveIngredient;
using Application.IServices.Medication;
using Application.IServices.MedicationClass;
using Application.IServices.MedicationForm;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace PharmacyManagmentApp.Controllers
{

    [Authorize(Roles = "Pharmacist",AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    [Route("api/medications")]
    public class MedicationsController : Controller
    {
        private readonly IMedicationService _medicationService;
        private readonly IMedicationFormService _medicationFormService;
        private readonly IMedicationClassService _medicationClassService;
        private readonly IActiveIngredientService _activeIngredientService;

        public MedicationsController(IMedicationService medicationService,IMedicationFormService medicationFormService,IActiveIngredientService activeIngredientService,IMedicationClassService medicationClassService)
        {
            _medicationService = medicationService;
            _medicationFormService = medicationFormService;
            _activeIngredientService = activeIngredientService;
            _medicationClassService = medicationClassService;
        }

        // * Medications * //
        [HttpGet]
        public async Task<IActionResult> GetAllMedications([FromQuery] string? search)
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
        public async Task<IActionResult> GetMedicationById(int id)
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
        public async Task<IActionResult> CreateMedication([FromBody] CreateMedicationDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                var newMedication = await _medicationService.CreateMedicationAsync(dto);
                return CreatedAtAction(nameof(GetMedicationById), new { id = newMedication.Id }, newMedication);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { Message = ex.Message });
            }
        }
        
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMedication(int id, [FromBody] UpdateMedicationDTO dto)
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

        

        //* MedicationForm *//

        [HttpGet("Forms")]
        public async Task<IActionResult> GetMedicationForms()
        {

            try
            {
                var result = await _medicationFormService.GetAllMedicationFormsAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Error = "Failed to retrieve medication forms",
                    Details = ex.Message
                });
            }
        }

        [HttpGet("Forms/{id}")]
        public async Task<IActionResult> GetMedicationFormById(int id)
        {
            try
            {
                var sup = await _medicationFormService.GetMedicationFormByIdAsync(id);
                if (sup == null) return NotFound(new { Error = $"Medication Form with ID {id} not found" });
                return Ok(sup);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Error = $"An error occurred while retrieving medication form with ID {id}",
                    Details = ex.Message
                });
            }
        }

        [HttpPost("Forms")]
        public async Task<IActionResult> CreateMedicationForm([FromBody] CreateMedicationFormDTO dto)
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
                var newForm = await _medicationFormService.CreateMedicationFormAsync(dto);
                return CreatedAtAction(nameof(GetMedicationFormById), new { id = newForm.Id }, newForm);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Error = "Failed to create medicationform",
                    Details = ex.Message
                });
            }

        }

        [HttpPut("Forms/{id}")]
        public async Task<IActionResult> UpdateMedicationForm(int id, [FromBody] UpdateMedicationFormDTO dto)
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
                await _medicationFormService.UpdateMedicationFormAsync(dto);
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
                    Error = $"An error occurred while updating medication form with ID {id}",
                    Details = ex.Message
                });
            }
        }

        [HttpDelete("Forms/{id}")]
        public async Task<IActionResult> DeleteMedicationForm(int id)
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
                await _medicationFormService.DeleteMedicationFormAsync(id);
                return Ok($"Deleted medication form with id {id}");
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
                    Error = $"An error occurred while Deleting Medication Form with ID {id}",
                    Details = ex.Message
                });
            }
        }




        //* MedicationClass *//

        [HttpGet("Classes")]
        public async Task<IActionResult> GetMedicationClasses()
        {

            try
            {
                var result = await _medicationFormService.GetAllMedicationFormsAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Error = "Failed to retrieve medication classes",
                    Details = ex.Message
                });
            }
        }

        [HttpGet("Classes/{id}")]
        public async Task<IActionResult> GetMedicationClassById(int id)
        {
            try
            {
                var sup = await _medicationClassService.GetMedicationClassByIdAsync(id);
                if (sup == null) return NotFound(new { Error = $"Medication Class with ID {id} not found" });
                return Ok(sup);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Error = $"An error occurred while retrieving medication class with ID {id}",
                    Details = ex.Message
                });
            }
        }

        [HttpPost("Classes")]
        public async Task<IActionResult> CreateMedicationClass([FromBody] CreateMedicationClassDTO dto)
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
                var newClass = await _medicationClassService.CreateMedicationClassAsync(dto);
                return CreatedAtAction(nameof(GetMedicationClassById), new { id = newClass.Id }, newClass);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Error = "Failed to create medication class",
                    Details = ex.Message
                });
            }

        }

        [HttpPut("Classes/{id}")]
        public async Task<IActionResult> UpdateMedicationClass(int id, [FromBody] UpdateMedicationClassDTO dto)
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
                await _medicationClassService.UpdateMedicationClassAsync(dto);
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
                    Error = $"An error occurred while updating medication class with ID {id}",
                    Details = ex.Message
                });
            }
        }

        [HttpDelete("Classes/{id}")]
        public async Task<IActionResult> DeleteMedicationClass(int id)
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
                await _medicationClassService.DeleteMedicationClassAsync(id);
                return Ok($"Deleted medication class with id {id}");
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
                    Error = $"An error occurred while Deleting Medication Class with ID {id}",
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

        [HttpGet("ActiveIngredients/{id}")]
        public async Task<IActionResult> GetActiveIngredientById(int id)
        {
            try
            {
                var ing = await _activeIngredientService.GetActiveIngredientByIdAsync(id);
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

        [HttpPost("ActiveIngredients")]
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

        [HttpPut("ActiveIngredients/{id}")]
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

        [HttpDelete("ActiveIngredients/{id}")]
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
