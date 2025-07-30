using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.IRepositories
{
    public interface ISaleRepository : IRepository<Sale, int>
    {
        Task<Sale> GetSaleWithDetailsAsync(int id);
    }
}
