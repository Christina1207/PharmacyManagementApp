using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class MedicationClass
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<Medication> Medications { get; set; } = new List<Medication>();
}
