using Domain.Entities;

namespace Domain.IRepositories
{
    public interface IMedicationRepository : IRepository<Medication, int>
    {
        public Task<Medication> GetByIdWithDetailsAsync(int id);
        public Task<IEnumerable<Medication>> GetAllWithDetailsAsync();
    }
}
