using System;
using System.Collections.Generic;

namespace Infrastructure.Data.Entities;

public partial class Sale
{
    public int Id { get; set; }

    public decimal TotalAmount { get; set; }

    public decimal? Discount { get; set; }

    public decimal AmountReceived { get; set; }

    public int PrescriptionId { get; set; }

    public int UserId { get; set; }

    public virtual Prescription Prescription { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
