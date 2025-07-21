using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class InsuredPerson
{
    public int Id { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public DateOnly DateOfBirth { get; set; }

    public string Status { get; set; } = null!;

    public bool Type { get; set; }

    public virtual Employee? Employee { get; set; }

    public virtual FamilyMember? FamilyMember { get; set; }

    public virtual ICollection<Prescription> Prescriptions { get; set; } = new List<Prescription>();
}
