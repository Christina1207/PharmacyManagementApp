using Application.DTOs.Department;

namespace Application.IServices.Department
{
    public interface IDepartmentService
    {
        public Task<GetDepartmentDTO> CreateDepartmentAsync(CreateDepartmentDTO dto);

        public Task UpdateDepartmentAsync(UpdateDepartmentDTO dto);
        public Task DeleteDepartmentAsync(int id);
        Task<IEnumerable<GetDepartmentDTO>> GetAllDepartmentsAsync();
        Task<GetDepartmentDTO> GetDepartmentByIdAsync(int id);

    }
}
