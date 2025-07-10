using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class OrderItem
{
    public int Id { get; set; }

    public int Quantity { get; set; }

    public DateOnly ExpirationDate { get; set; }

    public decimal UnitPrice { get; set; }

    public int OrderId { get; set; }

    public int MedicationId { get; set; }

    public virtual Medication Medication { get; set; } = null!;

    public virtual Order Order { get; set; } = null!;
}
