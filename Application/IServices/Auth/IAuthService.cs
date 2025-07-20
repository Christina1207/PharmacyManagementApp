using Application.DTOs.Auth;
using Microsoft.AspNetCore.Identity;

namespace Application.IServices.Auth
{
    public interface IAuthService
    {
        Task<IdentityResult> RegisterAsync(RegisterDTO registerDto, string role);
        Task<AuthResponseDTO> LoginAsync(LoginDTO loginDto);
        Task LogoutAsync();



    }
}
