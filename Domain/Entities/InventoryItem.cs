using System;
using System.Collections.Generic;

namespace Infrastructure.Data.Entities;

public partial class InventoryItem
{
    public int Id { get; set; }

    public decimal Price { get; set; }

    public int MedicationId { get; set; }

    public virtual ICollection<InventoryItemDetail> InventoryItemDetails { get; set; } = new List<InventoryItemDetail>();

    public virtual Medication Medication { get; set; } = null!;
}
