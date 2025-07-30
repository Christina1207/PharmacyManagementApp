using Application.DTOs.Sale;
using Application.IServices.Sale;
using AutoMapper;
using Domain.IUnitOfWork;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class SaleService : ISaleService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<SaleService> _logger;

        public SaleService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<SaleService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<GetSaleDTO>> GetAllSalesAsync()
        {
            _logger.LogInformation("Retrieving all sales records.");
            var sales = await _unitOfWork.Sales.GetAllAsync(s => s.User, s => s.Prescription);
            return _mapper.Map<IEnumerable<GetSaleDTO>>(sales);
        }

        public async Task<GetSaleDetailsDTO> GetSaleByIdAsync(int id) // Changed return type
        {
            _logger.LogInformation("Retrieving detailed sale record with ID {SaleId}", id);
            var sale = await _unitOfWork.Sales.GetSaleWithDetailsAsync(id); // Use new method
            if (sale == null)
            {
                throw new KeyNotFoundException($"Sale with ID {id} not found.");
            }
            return _mapper.Map<GetSaleDetailsDTO>(sale);
        }
    }
}
