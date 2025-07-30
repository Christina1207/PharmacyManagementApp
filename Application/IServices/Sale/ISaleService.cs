using Application.DTOs.Sale;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.IServices.Sale
{
    public interface ISaleService
    {
        Task<IEnumerable<GetSaleDTO>> GetAllSalesAsync();
        Task<GetSaleDetailsDTO> GetSaleByIdAsync(int id);
    }
}
