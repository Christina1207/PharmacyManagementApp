using Application.DTOs.Employee;
using Application.DTOs.InsuredPerson;
using Application.IServices.Department;
using Application.IServices.Employee;
using Application.IServices.InsuredPerson;
using AutoMapper;
using Domain.Entities;
using Domain.IRepositories;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace Application.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IRepository<Employee, int> _employeeRepository;
        private readonly IRepository<InsuredPerson, int> _insuredPersonRepository;
        private readonly IDepartmentService _departmentService;
        private readonly IInsuredPersonService _insuredPersonService;
        private readonly IMapper _mapper;
        private readonly ILogger<EmployeeService> _logger;
        public EmployeeService(IRepository<Employee, int> employeeRepository, IRepository<InsuredPerson, int> insuredPersonRepository, IMapper mapper, ILogger<EmployeeService> logger, IDepartmentService departmentService, IInsuredPersonService insuredPersonService)
        {
            _employeeRepository = employeeRepository;
            _departmentService = departmentService;
            _insuredPersonService = insuredPersonService;
            _mapper = mapper;
            _logger = logger;
            _insuredPersonRepository = insuredPersonRepository;

        }

        public  async Task<GetEmployeeDTO> CreateEmployeeAsync(CreateEmployeeDTO dto)
        {
            var department = await _departmentService.GetDepartmentByIdAsync(dto.DepartmentId)
                ?? throw new ArgumentException($"Department with ID {dto.DepartmentId} was not found.");

           
            CreateInsuredPersonDTO personDTO = new CreateInsuredPersonDTO
            {
                Type = false, //employee
                DateOfBirth = dto.DateOfBirth,
                FirstName = dto.FirstName,
                LastName = dto.LastName
            };

            var createdInsuredPerson = await _insuredPersonService.CreateInsuredPersonAsync(personDTO);
            var employee = _mapper.Map<Employee>(dto);

            employee.InsuredPersonId = createdInsuredPerson.Id;
            
            await _employeeRepository.AddAsync(employee);
            await _employeeRepository.SaveAsync();
            return _mapper.Map<GetEmployeeDTO>(employee);
        }

        public async Task DeleteEmployeeAsync(int id)
        {
            await _employeeRepository.DeleteByIdAsync(id);
            await _insuredPersonService.DeleteInsuredPersonAsync(id);
            await _employeeRepository.SaveAsync();
        }

        public async Task<IEnumerable<GetEmployeeDTO>> GetAllEmployeesAsync()
        {
            IEnumerable<Employee> employees = await _employeeRepository.GetAllAsync();
            if (employees is not null)
                foreach (var emp in employees)
                {
                    emp.InsuredPerson = _mapper.Map<InsuredPerson>(await _insuredPersonService.GetInsuredPersonByIdAsync(emp.InsuredPersonId));
                }

            return _mapper.Map<IEnumerable<GetEmployeeDTO>>(employees);
        }

        public async Task<GetEmployeeDTO> GetEmployeeByIdAsync(int id)
        {

            var dep = await _insuredPersonRepository.GetByIdAsync(id,i=>i.Employee)
             ?? throw new Exception($"employee {id} was not found");
            if (dep.Type.Equals(1)) throw new Exception($"employee {id} was not found");
            InsuredPerson per = _mapper.Map<InsuredPerson>(dep);
            var emp = per.Employee;

            if (emp is not null)
                emp.InsuredPerson = _mapper.Map<InsuredPerson>(await _insuredPersonService.GetInsuredPersonByIdAsync(emp.InsuredPersonId));


            return _mapper.Map<GetEmployeeDTO>(emp);
        }
    }
}
