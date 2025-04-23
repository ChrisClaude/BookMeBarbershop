using System;

namespace BookMe.Application.Entities;

public class Role : BaseEntity
{
    public string Name { get; set; }
    public IEnumerable<UserRole> UserRoles { get; set; }
}
