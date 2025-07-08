using System;
using System.Collections.Generic;

namespace Infrastructure.Data.Entities;

public partial class InventoryCheckItem
{
    public int Id { get; set; }

    public int ExpectedQuantity { get; set; }

    public int CountedQuantity { get; set; }

    public int InventoryCheckId { get; set; }

    public int MedicationId { get; set; }

    public virtual InventoryCheck InventoryCheck { get; set; } = null!;

    public virtual Medication Medication { get; set; } = null!;
}
