using Application.DTOs.Employee;
using Application.DTOs.FamilyMember;
using Application.DTOs.InsuredPerson;
using Domain.Entities;

namespace Application.IServices.Patient
{
    public interface IPatientService
    {
        Task<IEnumerable<GetInsuredPersonDTO>> GetAllPatientsAsync();
        Task<GetInsuredPersonDTO> GetPatientByIdAsync(int id);
        Task<GetEmployeeDTO> CreateEmployeeAsync(CreateEmployeeDTO dto);
        Task <GetFamilyMemberDTO>AddFamilyMemberToEmployeeAsync(CreateFamilyMemberDTO dto, int EmployeeId);

        Task DeactivatePatientAsync(int patientId);
        Task ActivatePatientAsync(int patientId);
        Task<IEnumerable<GetFamilyMemberDTO>> GetEmployeeFamilyMembersAsync(int id);




    }
}
