using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class Supplier
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? PhoneNumber { get; set; }
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
