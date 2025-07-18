using Application.DTOs.MedicationForm;

namespace Application.IServices.MedicationForm
{
    public interface IMedicationFormService
    {
        public Task<GetMedicationFormDTO> CreateMedicationFormAsync(CreateMedicationFormDTO dto);

        public Task UpdateMedicationFormAsync(UpdateMedicationFormDTO dto);
        public Task DeleteMedicationFormAsync(int id);
        Task<IEnumerable<GetMedicationFormDTO>> GetAllMedicationFormsAsync();
        Task<GetMedicationFormDTO> GetMedicationFormByIdAsync(int id);
    }
}
