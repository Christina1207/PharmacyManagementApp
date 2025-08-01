using Application.DTOs.User;
using Application.IServices.User;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;
        private readonly ILogger<UserService> _logger;

        public UserService(UserManager<User> userManager, ILogger<UserService> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<IEnumerable<UserDTO>> GetAllUsersAsync()
        {
            var users = await _userManager.Users.ToListAsync();
            var userDtos = new List<UserDTO>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userDtos.Add(new UserDTO
                {
                    Id = user.Id,
                    UserName = user.UserName!,
                    Email = user.Email!,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Role = roles.FirstOrDefault() ?? "No Role",
                    IsActive = user.IsActive,
                    CreatedAt = user.CreatedAt
                });
            }
            return userDtos;
        }

        public async Task<UserDTO> GetUserByIdAsync(int id)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null) throw new KeyNotFoundException($"User with ID {id} not found.");

            var roles = await _userManager.GetRolesAsync(user);
            return new UserDTO
            {
                Id = user.Id,
                UserName = user.UserName!,
                Email = user.Email!,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = roles.FirstOrDefault() ?? "No Role",
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt
            };
        }

        public async Task<bool> UpdateUserAsync(int id, UpdateUserDTO dto)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null) throw new KeyNotFoundException($"User with ID {id} not found.");

            user.IsActive = dto.IsActive;

            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);
            await _userManager.AddToRoleAsync(user, dto.Role);

            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }

        public async Task<bool> ActivateUserAsync(int id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null) throw new KeyNotFoundException($"User with ID {id} not found.");

            user.IsActive = true;
            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }

        public async Task<bool> DeactivateUserAsync(int id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null) throw new KeyNotFoundException($"User with ID {id} not found.");

            user.IsActive = false;
            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }

        public async Task<bool> ResetPasswordAsync(int id, string newPassword)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
            {
                throw new KeyNotFoundException($"User with ID {id} not found.");
            }

            // Generate a password reset token
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            // Reset the password using the token
            var result = await _userManager.ResetPasswordAsync(user, token, newPassword);

            return result.Succeeded;
        }
    }
}
