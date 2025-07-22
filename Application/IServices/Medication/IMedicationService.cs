using Application.DTOs.Medication;

namespace Application.IServices.Medication
{
    public interface IMedicationService
    {
        public Task<IEnumerable<GetMedicationDTO>> GetAllMedicationsAsync();
        public Task<GetMedicationDTO> GetMedicationByIdAsync(int id);
        public Task<GetMedicationDTO> CreateMedicationAsync(CreateMedicationDTO dto);
        public Task UpdateMedicationAsync(UpdateMedicationDTO dto);
        public Task DeleteMedicationAsync(int id);
    }
}
