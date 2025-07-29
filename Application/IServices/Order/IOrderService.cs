using Application.DTOs.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.IServices.Order
{
    public interface IOrderService
    {
        public Task<GetOrderDTO> CreateOrderAsync(CreateOrderDTO dto,int userId);
        public Task<GetOrderDTO> GetOrderByIdAsync(int id);
        public Task<IEnumerable<GetOrderDTO>> GetAllOrdersAsync();

    }
}
