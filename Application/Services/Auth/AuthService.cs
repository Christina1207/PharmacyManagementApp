using Application.DTOs.Auth;
using Application.IServices.Auth;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Application.Services.Auth
{
    public class AuthService:IAuthService
    {
        private readonly UserManager<User> _userManager;
        
        private readonly IConfiguration _configuration;

        public AuthService (UserManager<User> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        public async Task<AuthResponseDTO> LoginAsync(LoginDTO loginDto)
        {

            var user = await _userManager.FindByNameAsync(loginDto.Username!);
            if (user == null || !await _userManager.CheckPasswordAsync(user, loginDto.Password!))
            {
                // For security, i didn't specify whether the username or password was wrong.
                throw new UnauthorizedAccessException("Invalid username or password.");
            }
            if (!user.IsActive)
            {
                throw new UnauthorizedAccessException("This account has been deactivated.");
            }

            var userRoles = await _userManager.GetRolesAsync(user);

            var authClaims = new List<Claim>
            {
                new(ClaimTypes.Name, user.UserName !),
                 new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            foreach (var userRole in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, userRole));
            }

            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                expires: DateTime.Now.AddHours(3),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

            return new AuthResponseDTO
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expiration = token.ValidTo,
                User = new UserInfoDTO
                {
                    Username = user.UserName,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Role = userRoles.FirstOrDefault() 
                }
            };
        }

        public async Task LogoutAsync()
        {
            await Task.CompletedTask;
        }

    }
}
