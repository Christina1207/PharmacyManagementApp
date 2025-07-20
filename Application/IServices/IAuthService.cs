using Application.DTOs.Auth;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Application.IServices
{
    public interface IAuthService
    {
        Task<IdentityResult> RegisterAsync(RegisterDTO registerDto, string role);
        Task<AuthResponseDTO> LoginAsync(LoginDTO loginDto);



    }
}
