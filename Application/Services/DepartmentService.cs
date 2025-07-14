using Application.DTOs.Department;
using Application.IServices.Department;
using AutoMapper;
using Domain.Entities;
using Domain.IRepositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Services
{
    public class DepartmentService:IDepartmentService
    {

        private readonly IRepository<Department, int> _departmentRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<DepartmentService> _logger;

        public DepartmentService(IRepository<Department, int> departmentRepository, IMapper mapper, ILogger<DepartmentService> logger)
        {
            _departmentRepository = departmentRepository ?? throw new ArgumentNullException(nameof(departmentRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<GetDepartmentDTO> CreateDepartmentAsync(CreateDepartmentDTO dto)
        {
            _logger.LogInformation("Attmpting to create department: {DepartmentName}", dto.Name);
            try
            {
                if (dto is null)
                {
                    _logger.LogError("CreateDepartmentAsync called with null DTO.");
                    throw new ArgumentNullException(nameof(dto), "Department DTO cannot be null");
                }
                var existingDepartment = await _departmentRepository.GetByPredicateAsync(s => dto.Name!.Equals(s.Name));
                if (existingDepartment is not null)
                {
                    _logger.LogWarning("Department with name {DepartmentName} alreay exists.Creation FAILED.", dto.Name);
                    throw new InvalidOperationException($"Department with name '{dto.Name}' already exists.");

                }
                Department department = _mapper.Map<Department>(dto);
                await _departmentRepository.AddAsync(department);
                await _departmentRepository.SaveAsync();

                _logger.LogInformation("Department '{DepartmentName}' (ID: {DepartmentId}) created successfully.", department.Name, department.Id);
                return _mapper.Map<GetDepartmentDTO>(department);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while adding department with Name: {DepartmentName}", dto.Name);
                throw;
            }
        }

        public async Task DeleteDepartmentAsync(int id)
        {
            _logger.LogInformation("Attempting to delete department with ID: {DepartmentId}", id);
            try
            {
                var department = await _departmentRepository.GetByIdAsync(id);
                if (department is null)
                {
                    _logger.LogWarning("Department with ID: {DepartmentId} not found for deletion.", id);
                    throw new KeyNotFoundException($"Department with ID '{id}' not found.");
                }
                if (department.Employees is not null && department.Employees.Count != 0)
                {
                    _logger.LogWarning("Can't delete Department with ID: {DepartmentId} This Department is a foriegn key in Employees.", id);
                    throw new DbUpdateException($"Department with ID '{id}' is used by an employee or more.");
                }
                _departmentRepository.Delete(department);
                await _departmentRepository.SaveAsync();

                _logger.LogInformation("Department with ID: {DepartmentId} deleted successfully.", id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting department with ID {Id}.", id);
                throw;
            }

        }

        public async Task<IEnumerable<GetDepartmentDTO>> GetAllDepartmentsAsync()
        {
            _logger.LogInformation("Attempting to retreive all departments ");
            try
            {
                var departments = await _departmentRepository.GetAllAsync();
                _logger.LogInformation("Retreived all departments successfully");
                return _mapper.Map<IEnumerable<GetDepartmentDTO>>(departments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occured while retreiving all departments");
                throw;
            }
        }

        public async Task<GetDepartmentDTO> GetDepartmentByIdAsync(int id)
        {
            _logger.LogInformation("Attempting to retrieve department with ID: {DepartmentId}", id);
            try
            {
                var department = await _departmentRepository.GetByIdAsync(id);

                if (department is null)
                {
                    _logger.LogWarning("Department with ID: {DepartmentId} not found.", id);
                    throw new KeyNotFoundException($"Department with ID '{id}' not found.");
                }

                _logger.LogInformation("Department with ID: {DepartmentId} retrieved successfully.", id);
                return _mapper.Map<GetDepartmentDTO>(department);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while retrieving department with id: {DepartmentID}.", id);
                throw;
            }

        }

        public async Task UpdateDepartmentAsync(UpdateDepartmentDTO dto)
        {
            _logger.LogInformation("Attempting to update department with ID: {DepartmentId}", dto.Id);
            if (dto is null)
            {
                _logger.LogWarning("UpdateDepartmentAsync called with null DTO.");
                throw new ArgumentNullException(nameof(dto), "Department update DTO cannot be null.");
            }
            try
            {
                var department = await _departmentRepository.GetByIdAsync(dto.Id);
                if (department is null)
                {
                    _logger.LogWarning("Department with ID: {DepartmentId} not found for update.", dto.Id);
                    throw new KeyNotFoundException($"Department with ID '{dto.Id}' not found.");
                }

                // if name is updated , we check for uniqueness
                if (!department.Name!.Equals(dto.Name, StringComparison.OrdinalIgnoreCase))
                {
                    var departmentWithSameName = await _departmentRepository.GetByPredicateAsync(c => c.Name!.Equals(dto.Name) && c.Id != dto.Id);
                    if (departmentWithSameName is not null)
                    {
                        _logger.LogWarning("Another department with name '{DepartmentName}' already exists. Update failed for ID: {DepartmentId}.", dto.Name, dto.Id);
                        throw new InvalidOperationException($"Another department with name '{dto.Name}' already exists.");
                    }
                }
                _mapper.Map(dto, department);
                _departmentRepository.Update(department);
                await _departmentRepository.SaveAsync();

                _logger.LogInformation("Department '{Name}' updated successfully.", department.Name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating department with ID {Id}.", dto.Id);
                throw;
            }
        }
    }
}

