using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.FamilyMember
{
    public class GetFamilyMemberDTO
    {
        public int Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateOnly DateOfBirth { get; set; }
        public string? Status { get; set; }
        public int EmployeeId { get; set; }
        public string Relationship { get; set; } = null!;

    }
}
