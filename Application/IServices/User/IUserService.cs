using Application.DTOs.User;

namespace Application.IServices.User
{
    public interface IUserService
    {
        Task<IEnumerable<UserDTO>> GetAllUsersAsync();
        Task<UserDTO> GetUserByIdAsync(int id);
        Task<bool> UpdateUserAsync(int id, UpdateUserDTO dto);
        Task<bool> ActivateUserAsync(int id);
        Task<bool> DeactivateUserAsync(int id);
        Task<bool> ResetPasswordAsync(int id, string newPassword);
    }
}
