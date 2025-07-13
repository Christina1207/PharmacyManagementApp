using Application.DTOs.Supplier;


namespace Application.IServices
{
    public interface ISupplierService
    {
        public Task<GetSupplierDTO> CreateSupplierAsync(CreateSupplierDTO dto);

        public Task UpdateSupplierAsync(UpdateSupplierDTO dto);
        public Task DeleteSupplierAsync(int id);
        Task<IEnumerable<GetSupplierDTO>> GetAllSuppliersAsync();
        Task<GetSupplierDTO> GetSupplierByIdAsync(int id);


    }
}