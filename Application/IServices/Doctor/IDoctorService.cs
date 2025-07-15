using Application.DTOs.Doctor;


namespace Application.IServices.Doctor
{
    public interface IDoctorService
    {
        public Task<GetDoctorDTO> CreateDoctorAsync(CreateDoctorDTO dto);

        public Task UpdateDoctorAsync(UpdateDoctorDTO dto);
        public Task DeleteDoctorAsync(int id);
        Task<IEnumerable<GetDoctorDTO>> GetAllDoctorsAsync();
        Task<GetDoctorDTO> GetDoctorByIdAsync(int id);
    }
}
