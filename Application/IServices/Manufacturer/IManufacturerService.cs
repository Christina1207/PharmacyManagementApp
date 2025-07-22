using Application.DTOs.Manufacturer;

namespace Application.IServices.Manufacturer
{
    public interface IManufacturerService
    {
        public Task<IEnumerable<GetManufacturerDTO>> GetAllManufacturersAsync();
        public Task<GetManufacturerDTO> GetManufacturerByIdAsync(int id);
        public Task<GetManufacturerDTO> CreateManufacturerAsync(CreateManufacturerDTO dto);
        public Task UpdateManufacturerAsync(UpdateManufacturerDTO dto);
        public Task DeleteManufacturerAsync(int id);
    }
}
