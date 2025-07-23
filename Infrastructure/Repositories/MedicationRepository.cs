using Domain.Entities;
using Domain.IRepositories;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class MedicationRepository : Repository<Medication, int>, IMedicationRepository
    {

        public MedicationRepository(PharmacyDbContext context) : base(context)
        { 
        }

        public async Task<Medication> GetByIdWithDetailsAsync(int id)
        {
            return await _dbSet.Where(m => m.Id == id)
             .Include(m => m.Manufacturer)
             .Include(m => m.Form)
             .Include(m => m.Class)
             .Include(m => m.MedicationActiveIngredients)
             .ThenInclude(mai => mai.Ingredient)
             .FirstOrDefaultAsync();

        }

        public async Task<IEnumerable<Medication>> GetAllWithDetailsAsync()
        {
            return await _dbSet
            .Include(m => m.Manufacturer)
            .Include(m => m.Form)
            .Include(m => m.Class)
            .Include(m => m.MedicationActiveIngredients)
            .ThenInclude(mai => mai.Ingredient)
            .ToListAsync();
        }
    }
}
