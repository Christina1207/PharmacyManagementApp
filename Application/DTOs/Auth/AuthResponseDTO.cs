
namespace Application.DTOs.Auth
{
    public class AuthResponseDTO
    {
        public string? Token { get; set; }
        public DateTime Expiration { get; set; }
        public string? Username { get; set; }
    }
}
