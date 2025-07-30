using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.IRepositories
{
    public interface IInventoryCheckRepository : IRepository<InventoryCheck, int>
    {
        Task<InventoryCheck> GetCheckWithDetailsAsync(int id);
        Task<IEnumerable<InventoryCheck>> GetAllChecksWithDetailsAsync();
    }
}
