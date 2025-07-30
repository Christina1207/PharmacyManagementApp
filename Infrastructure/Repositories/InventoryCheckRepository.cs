using Domain.Entities;
using Domain.IRepositories;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class InventoryCheckRepository : Repository<InventoryCheck, int>, IInventoryCheckRepository
    {
        public InventoryCheckRepository(PharmacyDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<InventoryCheck>> GetAllChecksWithDetailsAsync()
        {
            return await _dbSet
                .Include(ic => ic.User)
                .Include(ic => ic.InventoryCheckItems)
                    .ThenInclude(ici => ici.Medication) // Correctly load Medication within each Item
                .ToListAsync();
        }

        public async Task<InventoryCheck> GetCheckWithDetailsAsync(int id)
        {
            return await _dbSet
                .Where(ic => ic.Id == id)
                .Include(ic => ic.User)
                .Include(ic => ic.InventoryCheckItems)
                    .ThenInclude(ici => ici.Medication) // Correctly load Medication within each Item
                .FirstOrDefaultAsync();
        }
    }
}
