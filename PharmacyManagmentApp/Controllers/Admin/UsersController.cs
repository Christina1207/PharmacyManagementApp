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

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var success = await _userService.UpdateUserAsync(id, dto);
                if (success) return NoContent();
                return BadRequest("Failed to update user.");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
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
    }
}
