using Domain.Entities;
using Domain.IRepositories;


namespace Domain.IUnitOfWork
{
    public interface IUnitOfWork:IDisposable
    {
        IRepository<Prescription, int> Prescriptions { get; }
        IRepository<InventoryItem, int> InventoryItems { get; }
        IRepository<InventoryItemDetail, int> InventoryItemDetails { get; }
        IRepository<InsuredPerson, int> InsuredPersons { get; }
        ISaleRepository Sales { get; }
        IOrderRepository Orders { get; }
        IRepository<Medication,int> Medications { get; }

        // inventory checks
        IInventoryCheckRepository InventoryChecks { get; }
        Task<int> SaveChangesAsync();
    }
}
