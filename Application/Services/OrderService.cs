using Application.DTOs.Inventory;
using Application.DTOs.Order;
using Application.IServices.IInventoryService;
using Application.IServices.Order;
using AutoMapper;
using Domain.Entities;
using Domain.IUnitOfWork;
using Microsoft.Extensions.Logging;


namespace Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IInventoryService _inventoryService;
        private readonly ILogger<OrderService> _logger;

        public OrderService(IUnitOfWork unitOfWork, IMapper mapper, IInventoryService inventoryService, ILogger<OrderService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _inventoryService = inventoryService;
            _logger = logger;
        }

        public async Task<GetOrderDTO> CreateOrderAsync(CreateOrderDTO dto, int userId)
        {
            _logger.LogInformation("Processing new order from supplier {SupplierId}", dto.SupplierId);

            var order = _mapper.Map<Order>(dto);
            order.UserId = userId;
            order.TotalValue = dto.OrderItems.Sum(item => item.Quantity * item.UnitPrice);

            await _unitOfWork.Orders.AddAsync(order);
            await _unitOfWork.SaveChangesAsync(); 

            _logger.LogInformation("Order {OrderId} created. Now updating inventory stock.", order.Id);

            foreach (var item in dto.OrderItems)
            {
                var stockDto = new AddStockDTO
                {
                    MedicationId = item.MedicationId,
                    Quantity = item.Quantity,
                    Price = item.UnitPrice, // Use the price from the order item
                    ExpirationDate = item.ExpirationDate
                };
                // This service call handles all the complex inventory logic automatically
                await _inventoryService.AddStockAsync(stockDto);
            }

            _logger.LogInformation("Inventory updated successfully for order {OrderId}", order.Id);

            return await GetOrderByIdAsync(order.Id);
        }

        public async Task<GetOrderDTO> GetOrderByIdAsync(int id)
        {
            var order = await _unitOfWork.Orders.GetOrderWithDetailsAsync(id); // Use the new repository method
            if (order == null)
            {
                throw new KeyNotFoundException($"Order with ID {id} not found.");
            }
            return _mapper.Map<GetOrderDTO>(order);
        }

        public async Task<IEnumerable<GetOrderDTO>> GetAllOrdersAsync()
        {
            var orders = await _unitOfWork.Orders.GetAllOrdersWithDetailsAsync(); // Use the new repository method
            return _mapper.Map<IEnumerable<GetOrderDTO>>(orders);
        }

    }
}