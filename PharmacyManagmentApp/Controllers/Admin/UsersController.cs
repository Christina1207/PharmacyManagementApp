using Application.DTOs.Auth;
using Application.DTOs.User;
using Application.IServices.User;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PharmacyManagmentApp.Controllers.Admin
{
    [Authorize(Roles = "Admin", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    [Route("api/admin/users")]
    public class UsersController : Controller
    {
        private readonly IUserService _userService;
        public UsersController(IUserService userService) {_userService = userService; }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }


        //unused (remove later)
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            try
            {
                var user = await _userService.GetUserByIdAsync(id);
                return Ok(user);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { ex.Message });
            }
        }

        [HttpPut("{id}/activate")]
        public async Task<IActionResult> ActivateUser(int id)
        {
            try
            {
                var success = await _userService.ActivateUserAsync(id);
                if (success) return NoContent();
                return BadRequest("Failed to activate user.");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { ex.Message });
            }
        }

        [HttpPut("{id}/deactivate")]
        public async Task<IActionResult> DeactivateUser(int id)
        {
            try
            {
                var success = await _userService.DeactivateUserAsync(id);
                if (success) return NoContent();
                return BadRequest("Failed to deactivate user.");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { ex.Message });
            }
        }

        [HttpPost("{id}/reset-password")]
        public async Task<IActionResult> ResetPassword(int id, [FromBody] PasswordResetDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var success = await _userService.ResetPasswordAsync(id, dto.NewPassword);
                if (success)
                {
                    return Ok(new { Message = "Password has been reset successfully." });
                }
                return BadRequest("Failed to reset password.");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { ex.Message });
            }
        }


        // * Register * //

        [HttpPost("register-pharmacist")]
        public async Task<IActionResult> RegisterPharmacist([FromBody] RegisterDTO model)
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
            var result = await _userService.RegisterAsync(model, "Pharmacist");

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return Ok(new { Status = "Success", Message = "Pharmacist registered successfully!" });
        }


        [HttpPost("register-admin")]
        public async Task<IActionResult> RegisterAdmin([FromBody] RegisterDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _userService.RegisterAsync(dto, "Admin");

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return Ok(new { Status = "Success", Message = "Admin registered successfully!" });
        }


    }
}
