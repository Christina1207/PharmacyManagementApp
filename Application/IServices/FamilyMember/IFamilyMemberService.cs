using Application.DTOs.FamilyMember;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.IServices.FamilyMember
{
    public interface IFamilyMemberService
    {
        public Task<GetFamilyMemberDTO> AddFamilyMemberToEmployeeAsync(int employeeId, CreateFamilyMemberDTO dto);
        public Task<IEnumerable<GetFamilyMemberDTO>> GetEmployeeFamilyMembersAsync(int employeeId);



    }
}
