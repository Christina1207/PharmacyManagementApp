using System;
using System.Collections.Generic;

namespace Infrastructure.Data.Entities;

public partial class InventoryCheck
{
    public int Id { get; set; }

    public DateTime Date { get; set; }

    public decimal TotalValue { get; set; }

    public string? Notes { get; set; }

    public int UserId { get; set; }

    public virtual ICollection<InventoryCheckItem> InventoryCheckItems { get; set; } = new List<InventoryCheckItem>();

    public virtual User User { get; set; } = null!;
}
