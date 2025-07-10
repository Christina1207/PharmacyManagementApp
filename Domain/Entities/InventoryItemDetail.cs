using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class InventoryItemDetail
{
    public int Id { get; set; }

    public int Quantity { get; set; }

    public DateOnly ExpirationDate { get; set; }

    public int ItemId { get; set; }

    public virtual InventoryItem Item { get; set; } = null!;
}
