
namespace Application.DTOs.Auth
{
    public class AuthResponseDTO
    {
        public string? Token { get; set; }
        public DateTime Expiration { get; set; }
        public UserInfoDTO? User { get; set; }
    }
    public class UserInfoDTO
    {
        public string? Username { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Role { get; set; }
    }
}
