using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class FamilyMember
{
    public int InsuredPersonId { get; set; }

    public int EmployeeId { get; set; }

    public string Relationship { get; set; } = null!;

    public virtual InsuredPerson InsuredPerson { get; set; } = null!;

    public virtual Employee Employee { get; set; } = null!;
}
