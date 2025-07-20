using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.FamilyMember
{
    public class CreateFamilyMemberDTO
    {
        [Required]
        public string? FirstName { get; set; }
        [Required]
        public string? LastName { get; set; }
        [Required]
        public DateOnly DateOfBirth { get; set; }
        [Required]
        public int EmployeeId { get; set; }

        public string Relationship { get; set; } = null!;

    }
}
