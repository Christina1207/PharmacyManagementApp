using Domain.Entities;


namespace Domain.IRepositories
{
    public interface IOrderRepository : IRepository<Order, int>
    {
        Task<Order> GetOrderWithDetailsAsync(int id);
        Task<IEnumerable<Order>> GetAllOrdersWithDetailsAsync();
    }
}
