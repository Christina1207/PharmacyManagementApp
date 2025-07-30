using Domain.Entities;
using Domain.IRepositories;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class SaleRepository : Repository<Sale, int>, ISaleRepository
    {
        public SaleRepository(PharmacyDbContext context) : base(context)
        {
        }

        public async Task<Sale> GetSaleWithDetailsAsync(int id)
        {
            return await _dbSet
                .Where(s => s.Id == id)
                .Include(s => s.User) // Pharmacist
                .Include(s => s.Prescription)
                    .ThenInclude(p => p.Patient) // Patient on the Prescription
                .Include(s => s.Prescription)
                    .ThenInclude(p => p.Doctor) // Doctor on the Prescription
                .Include(s => s.Prescription)
                    .ThenInclude(p => p.PrescriptionItems)
                        .ThenInclude(pi => pi.Medication) // Medications on the Prescription
                .FirstOrDefaultAsync();
        }
    }
}
