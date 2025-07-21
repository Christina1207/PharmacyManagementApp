using AutoMapper;
using Domain.Entities;
using Domain.IRepositories;
using Microsoft.Extensions.Logging;


namespace Application.Services
{
    public class EmployeeService
    {
        private readonly IRepository<Employee, int> _employeeRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<EmployeeService> _logger;

    }
}
