using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class MedicationForm
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Unit { get; set; } = null!;

    public virtual ICollection<Medication> Medications { get; set; } = new List<Medication>();
}
