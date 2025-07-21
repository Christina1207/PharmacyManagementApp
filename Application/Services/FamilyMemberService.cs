using Application.DTOs.Employee;
using Application.DTOs.FamilyMember;
using Application.DTOs.InsuredPerson;
using Application.IServices.Employee;
using Application.IServices.FamilyMember;
using Application.IServices.InsuredPerson;
using AutoMapper;
using Domain.Entities;
using Domain.IRepositories;
using Microsoft.Extensions.Logging;

namespace Application.Services
{
    public class FamilyMemberService : IFamilyMemberService
    {
        private readonly IRepository<FamilyMember, int> _familyMemberRepository;
        private readonly IEmployeeService _employeeService;
        private readonly IInsuredPersonService _insuredPersonService;
        private readonly ILogger<FamilyMemberService> _logger;
        private readonly IMapper _mapper;

        public FamilyMemberService(IInsuredPersonService insuredPersonService,
            IEmployeeService employeeService,
            IRepository<FamilyMember, int> familyMemberRepository,
            IMapper mapper,
            ILogger <FamilyMemberService> logger)
        {
            _familyMemberRepository = familyMemberRepository;
            _employeeService = employeeService;
            _logger = logger;
            _mapper = mapper;
            _insuredPersonService = insuredPersonService;
        }

        public async Task<GetFamilyMemberDTO> AddFamilyMemberToEmployeeAsync(int employeeId, CreateFamilyMemberDTO dto)
        {
            _logger.LogInformation("Attempting to add family member to employee ID: {EmployeeId}", employeeId);
            var employee = await _employeeService.GetEmployeeByIdAsync(employeeId);
            if (employee == null)
            {
                _logger.LogWarning("Employee with ID {EmployeeId} not found.", employeeId);
                throw new KeyNotFoundException($"Employee with ID {employeeId} not found.");
            }

            CreateInsuredPersonDTO personDTO = new CreateInsuredPersonDTO
            {
                Type = true, //family member
                DateOfBirth = dto.DateOfBirth,
                FirstName = dto.FirstName,
                LastName = dto.LastName
            };
            var createdInsuredPerson = await _insuredPersonService.CreateInsuredPersonAsync(personDTO);
            var familyMember = _mapper.Map<FamilyMember>(dto);

            familyMember.InsuredPersonId = createdInsuredPerson.Id;
            await _familyMemberRepository.AddAsync(familyMember);
            await _familyMemberRepository.SaveAsync();
            
            _logger.LogInformation("Successfully added family member {FirstName} {LastName} to employee ID: {EmployeeId}", dto.FirstName, dto.LastName, employeeId);

            return _mapper.Map<GetFamilyMemberDTO>(familyMember);
        }

        public async Task<IEnumerable<GetFamilyMemberDTO>> GetEmployeeFamilyMembersAsync(int employeeId)
        {
            _logger.LogInformation("Retrieving family members for employee ID: {EmployeeId}", employeeId);
            var employee = await _employeeService.GetEmployeeByIdAsync(employeeId);
            if (employee == null)
            {
                _logger.LogWarning("Employee with ID {EmployeeId} not found.", employeeId);
                throw new KeyNotFoundException($"Employee with ID {employeeId} not found.");
            }

            var familyMembers = await _familyMemberRepository.GetAllByPredicateAsync(fm => fm.EmployeeId == employeeId);
            return _mapper.Map<IEnumerable<GetFamilyMemberDTO>>(familyMembers);
        }
    }
}
