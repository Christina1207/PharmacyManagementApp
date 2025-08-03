using Application.DTOs.Auth;
using Microsoft.AspNetCore.Identity;

namespace Application.IServices.Auth
{
    public interface IAuthService
    {
        Task<AuthResponseDTO> LoginAsync(LoginDTO loginDto);
        Task LogoutAsync();


    }
}
