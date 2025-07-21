using Application.DTOs.InsuredPerson;

namespace Application.IServices.InsuredPerson
{
    public interface IInsuredPersonService
    {
        public Task<GetInsuredPersonDTO> CreateInsuredPersonAsync(CreateInsuredPersonDTO createInsuredPersonDto);
        public Task DeleteInsuredPersonAsync(int id);
        public Task<GetInsuredPersonDTO> GetInsuredPersonByIdAsync(int id);
        public Task<IEnumerable<GetInsuredPersonDTO>> GetAllInsuredPersonsAsync();
        public Task ActivateInsuredPersonAsync(int id);
        public Task DeactivateInsuredPersonAsync(int id);


    }
}
