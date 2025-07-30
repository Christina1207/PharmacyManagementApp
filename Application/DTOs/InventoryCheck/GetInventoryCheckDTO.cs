using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.InventoryCheck
{
    public class GetInventoryCheckDTO
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string? Notes { get; set; }
        public string? UserName { get; set; }
        public List<GetInventoryCheckItemDTO> InventoryCheckItems { get; set; } = new List<GetInventoryCheckItemDTO>();
    }
}
