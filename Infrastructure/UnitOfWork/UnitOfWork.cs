using Domain.Entities;
using Domain.IRepositories;
using Domain.IUnitOfWork;
using Infrastructure.Context;
using Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.UnitOfWork
{
    public class UnitOfWork :IUnitOfWork
    {
        private readonly PharmacyDbContext _context;
        public IRepository<Prescription, int> Prescriptions { get; private set; }
        public IRepository<InventoryItemDetail, int> InventoryItemDetails { get; private set; }
        public IRepository<Sale, int> Sales { get; private set; }

        public UnitOfWork(PharmacyDbContext context)
        {
            _context = context;
            Prescriptions = new Repository<Prescription, int>(_context);
            InventoryItemDetails = new Repository<InventoryItemDetail, int>(_context);
            Sales = new Repository<Sale, int>(_context);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
