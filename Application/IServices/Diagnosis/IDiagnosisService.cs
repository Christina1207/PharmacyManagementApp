using Application.DTOs.Diagnosis;


namespace Application.IServices.Diagnosis
{
    public interface IDiagnosisService
    {
        public Task<GetDiagnosisDTO> CreateDiagnosisAsync(CreateDiagnosisDTO dto);
        public Task UpdateDiagnosisAsync(UpdateDiagnosisDTO dto);
        public Task DeleteDiagnosisAsync(int id);
        Task<IEnumerable<GetDiagnosisDTO>> GetAllDiagnosesAsync();
        Task<GetDiagnosisDTO> GetDiagnosisByIdAsync(int id);

    }
}
