using Domain.Entities;
using Domain.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.IUnitOfWork
{
    public interface IUnitOfWork
    {
        IRepository<Prescription, int> Prescriptions { get; }
        IRepository<InventoryItemDetail, int> InventoryItemDetails { get; }
        IRepository<Sale, int> Sales { get; }

        Task<int> SaveChangesAsync();
    }
}
