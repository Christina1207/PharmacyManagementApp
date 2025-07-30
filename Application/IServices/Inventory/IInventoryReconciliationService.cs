using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.IServices.Inventory
{
    public interface IInventoryReconciliationService
    {
        Task ReconcileInventoryAsync(int inventoryCheckId);
    }
}
