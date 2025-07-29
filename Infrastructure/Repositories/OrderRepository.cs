using Domain.Entities;
using Domain.IRepositories;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class OrderRepository : Repository<Order, int>, IOrderRepository
    {
        public OrderRepository(PharmacyDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Order>> GetAllOrdersWithDetailsAsync()
        {
            return await _dbSet
                .Include(o => o.Supplier)
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Medication) // Correctly include nested entities
                .ToListAsync();
        }

        public async Task<Order> GetOrderWithDetailsAsync(int id)
        {
            return await _dbSet
                .Where(o => o.Id == id)
                .Include(o => o.Supplier)
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Medication) // Correctly include nested entities
                .FirstOrDefaultAsync();
        }
    }
}