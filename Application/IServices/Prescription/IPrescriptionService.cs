using Application.DTOs.Prescription;
using Application.DTOs.Sale;


namespace Application.IServices.Prescription
{
    public interface IPrescriptionService
    {
        public Task<GetSaleDTO> ProcessPrescriptionAsync(CreatePrescriptionDTO dto);
    }
}
