using System;
using System.Collections.Generic;

namespace Infrastructure.Data.Entities;

public partial class Order
{
    public int Id { get; set; }

    public decimal TotalValue { get; set; }

    public DateTime OrderDate { get; set; }

    public int SupplierId { get; set; }

    public int UserId { get; set; }

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual Supplier Supplier { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
