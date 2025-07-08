using System;
using System.Collections.Generic;

namespace Infrastructure.Data.Entities;

public partial class Manufacturer
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? PhoneNumber { get; set; }

    public virtual ICollection<Medication> Medications { get; set; } = new List<Medication>();
}
