using Application.DTOs.InsuredPerson;
using Application.IServices.Patient;
using AutoMapper;
using Domain.Entities;
using Domain.IRepositories;
using Microsoft.Extensions.Logging;

namespace Application.Services
{
    public class PatientService : IPatientService
    {
        private readonly IRepository<Employee, int> _employeeRepository;
        private readonly IRepository<InsuredPerson, int> _insuredPersonRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<PatientService> _logger;

        public PatientService(IRepository<Employee, int> employeeRepository, IRepository<InsuredPerson, int> insuredPersonRepository, IMapper mapper, ILogger<PatientService> logger)
        {
            _employeeRepository = employeeRepository;
            _insuredPersonRepository = insuredPersonRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<GetInsuredPersonDTO> CreateEmployeeAsync(CreateEmployeeDTO dto)
        {
            _logger.LogInformation("Attempting to create a new employee: {FirstName} {LastName}", dto.FirstName, dto.LastName);
            if (dto == null)
            {
                throw new ArgumentNullException(nameof(dto), "Employee creation data cannot be null.");
            }

            try
            {
                // Step 1: Create the InsuredPerson record
                var insuredPerson = _mapper.Map<InsuredPerson>(dto);
                await _insuredPersonRepository.AddAsync(insuredPerson);
                await _insuredPersonRepository.SaveAsync(); // Save to get the new Id

                // Step 2: Create the Employee record linked to the InsuredPerson
                var employee = new Employee
                {
                    InsuredPersonId = insuredPerson.Id,
                    DepartmentId = dto.DepartmentId,
                    PhoneNumber = dto.PhoneNumber
                };
                await _employeeRepository.AddAsync(employee);
                await _employeeRepository.SaveAsync();

                // Manually load navigation properties for the DTO mapping
                var newEmployee = await _employeeRepository.GetByIdAsync(employee.InsuredPersonId);

                _logger.LogInformation("Successfully created employee with ID: {EmployeeId}", employee.InsuredPersonId);
                return _mapper.Map<GetInsuredPersonDTO>(newEmployee);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating employee {FirstName} {LastName}", dto.FirstName, dto.LastName);
                throw; // Rethrow to be handled by the controller
            }
        }

        public async Task<GetInsuredPersonDTO> GetPatientByIdAsync(int id)
        {
            _logger.LogInformation("Attempting to retrieve patient with ID: {PatientId}", id);
            var employee = await _employeeRepository.GetByIdAsync(id);
            if (employee == null)
            {
                _logger.LogWarning("Patient with ID {PatientId} not found.", id);
                throw new KeyNotFoundException($"Patient with ID {id} not found.");
            }
            return _mapper.Map<GetInsuredPersonDTO>(employee);
        }

        public async Task<IEnumerable<GetInsuredPersonDTO>> GetAllPatientsAsync()
        {
            _logger.LogInformation("Attempting to retrieve all patients");
            var employees = await _employeeRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<GetInsuredPersonDTO>>(employees);
        }
    }
}
