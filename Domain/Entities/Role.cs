using Microsoft.AspNetCore.Identity;

namespace Domain.Entities;

public partial class Role: IdentityRole<int>
{
    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
