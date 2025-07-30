using Application.DTOs.Reporting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.IServices.Reporting
{
    public interface IReportService
    {
        Task<FinancialReportDTO> GetFinancialReportAsync(DateTime startDate, DateTime endDate);
        Task<InventoryReportDTO> GetInventoryReportAsync();
    }

}
