using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class PrescriptionItem
{
    public int Id { get; set; }

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public int PrescriptionId { get; set; }

    public int MedicationId { get; set; }

    public virtual Medication Medication { get; set; } = null!;

    public virtual Prescription Prescription { get; set; } = null!;
}
