using Application.DTOs.Employee;

namespace Application.IServices.Employee
{
    public interface IEmployeeService
    {
        Task<GetEmployeeDTO> CreateEmployeeAsync(CreateEmployeeDTO dto);        
        Task DeleteEmployeeAsync(int id);
        Task<IEnumerable<GetEmployeeDTO>> GetAllEmployeesAsync();
        Task<GetEmployeeDTO> GetEmployeeByIdAsync(int id);
    }
}
