using Application.DTOs.Employee;
using Application.DTOs.FamilyMember;
using Application.DTOs.InsuredPerson;
using Application.IServices.Patient;
using AutoMapper;
using Domain.Entities;
using Domain.IRepositories;
using Microsoft.Extensions.Logging;

namespace Application.Services.Auth
{
    public class PatientService : IPatientService
    {
        private readonly IRepository<InsuredPerson, int> _insuredPersonRepository;
        private readonly IRepository<Employee, int> _employeeRepository;
        private readonly IRepository<FamilyMember, int> _familyMemberRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<PatientService> _logger;

        public PatientService(
            IRepository<InsuredPerson, int> insuredPersonRepository,
            IRepository<Employee, int> employeeRepository,
            IRepository<FamilyMember, int> familyMemberRepository,
            IMapper mapper,
            ILogger<PatientService> logger)
        {
            _insuredPersonRepository = insuredPersonRepository;
            _employeeRepository = employeeRepository;
            _familyMemberRepository = familyMemberRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<GetEmployeeDTO> CreateEmployeeAsync(CreateEmployeeDTO dto)
        {
            _logger.LogInformation("Attempting to create a new employee: {FirstName} {LastName}", dto.FirstName, dto.LastName);
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            var emp = await _employeeRepository

            var insuredPerson = _mapper.Map<InsuredPerson>(dto);
            await _insuredPersonRepository.AddAsync(insuredPerson);
            await _insuredPersonRepository.SaveAsync();

            var employee = new Employee
            {
                InsuredPersonId = insuredPerson.Id,
                DepartmentId = dto.DepartmentId,
                PhoneNumber = dto.PhoneNumber,
            };
            await _employeeRepository.AddAsync(employee);
            await _employeeRepository.SaveAsync();

            _logger.LogInformation("Successfully created employee with ID: {Id}", insuredPerson.Id);

            // Fetch the newly created employee with its navigation properties for correct mapping
            var newEmployee = await _employeeRepository.GetByPredicateAsync(e => e.InsuredPersonId == insuredPerson.Id);
            return _mapper.Map<GetEmployeeDTO>(newEmployee);
        }

        public async Task<GetFamilyMemberDTO> AddFamilyMemberToEmployeeAsync(int employeeId, CreateFamilyMemberDTO dto)
        {
            _logger.LogInformation("Attempting to add family member to employee ID: {EmployeeId}", employeeId);
            var employee = await _employeeRepository.GetByIdAsync(employeeId);
            if (employee == null)
            {
                _logger.LogWarning("Employee with ID {EmployeeId} not found.", employeeId);
                throw new KeyNotFoundException($"Employee with ID {employeeId} not found.");
            }

            var insuredPerson = _mapper.Map<InsuredPerson>(dto);
            await _insuredPersonRepository.AddAsync(insuredPerson);
            await _insuredPersonRepository.SaveAsync();

            var familyMember = new FamilyMember
            {
                InsuredPersonId = insuredPerson.Id,
                EmployeeId = employeeId,
                Relationship = dto.Relationship,
            };
            await _familyMemberRepository.AddAsync(familyMember);
            await _familyMemberRepository.SaveAsync();

            _logger.LogInformation("Successfully added family member {FirstName} {LastName} to employee ID: {EmployeeId}", dto.FirstName, dto.LastName, employeeId);

            var newFamilyMember = await _familyMemberRepository.GetByPredicateAsync(fm => fm.InsuredPersonId == insuredPerson.Id);
            return _mapper.Map<GetFamilyMemberDTO>(newFamilyMember);
        }

        public async Task<IEnumerable<GetInsuredPersonDTO>> GetAllPatientsAsync()
        {
            _logger.LogInformation("Retrieving all patients");
            var patients = await _insuredPersonRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<GetInsuredPersonDTO>>(patients);
        }

        public async Task<GetInsuredPersonDTO> GetPatientByIdAsync(int id)
        {
            _logger.LogInformation("Retrieving patient by ID: {Id}", id);
            var patient = await _insuredPersonRepository.GetByIdAsync(id);
            if (patient == null)
            {
                _logger.LogWarning("Patient with ID {Id} not found.", id);
                throw new KeyNotFoundException($"Patient with ID {id} not found.");
            }
            return _mapper.Map<GetInsuredPersonDTO>(patient);
        }

        public async Task<IEnumerable<GetFamilyMemberDTO>> GetEmployeeFamilyMembersAsync(int employeeId)
        {
            _logger.LogInformation("Retrieving family members for employee ID: {EmployeeId}", employeeId);
            var employee = await _employeeRepository.GetByIdAsync(employeeId);
            if (employee == null)
            {
                _logger.LogWarning("Employee with ID {EmployeeId} not found.", employeeId);
                throw new KeyNotFoundException($"Employee with ID {employeeId} not found.");
            }

            var familyMembers = await _familyMemberRepository.GetAllByPredicateAsync(fm => fm.EmployeeId == employeeId);
            return _mapper.Map<IEnumerable<GetFamilyMemberDTO>>(familyMembers);
        }

        public async Task DeactivatePatientAsync(int patientId)
        {
            await SetPatientStatusAsync(patientId, "Inactive");
        }

        public async Task ActivatePatientAsync(int patientId)
        {
            await SetPatientStatusAsync(patientId, "Active");
        }

        private async Task SetPatientStatusAsync(int patientId, string status)
        {
            _logger.LogInformation("Setting status to '{Status}' for patient ID: {PatientId}", status, patientId);
            var patient = await _insuredPersonRepository.GetByIdAsync(patientId);
            if (patient == null)
            {
                _logger.LogWarning("Patient with ID {PatientId} not found for status update.", patientId);
                throw new KeyNotFoundException($"Patient with ID {patientId} not found.");
            }

            patient.Status = status;
            _insuredPersonRepository.Update(patient);
            await _insuredPersonRepository.SaveAsync();
            _logger.LogInformation("Successfully updated status for patient ID: {PatientId}", patientId);
        }
    }
}
