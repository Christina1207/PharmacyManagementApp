using Domain.Entities;
using Domain.IRepositories;
using Domain.IUnitOfWork;
using Infrastructure.Context;
using Infrastructure.Repositories;

namespace Infrastructure.UnitOfWork
{
    public class UnitOfWork :IUnitOfWork
    {
        private readonly PharmacyDbContext _context;
        public IRepository<Prescription, int> Prescriptions { get; private set; }
        public IRepository<InventoryItem, int> InventoryItems { get; private set; }
        public IRepository<InventoryItemDetail, int> InventoryItemDetails { get; private set; }
        public IRepository<InsuredPerson, int> InsuredPersons { get; private set; }
        public IRepository<Sale, int> Sales { get; private set; }
        public IRepository<Medication, int> Medications { get; private set; }

        // order
        public IOrderRepository Orders { get; private set; }

        // inventory check
        public IInventoryCheckRepository InventoryChecks { get; private set; }
        public UnitOfWork(PharmacyDbContext context)
        {
            _context = context;
            Prescriptions = new Repository<Prescription, int>(_context);
            InventoryItems = new Repository<InventoryItem, int>(_context);
            InventoryItemDetails = new Repository<InventoryItemDetail, int>(_context);
            InsuredPersons = new Repository<InsuredPerson, int>(_context);
            Sales = new Repository<Sale, int>(_context);
            Orders = new OrderRepository(_context);
            Medications = new Repository<Medication, int>(_context);
            InventoryChecks = new InventoryCheckRepository(_context);

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
