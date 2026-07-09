using Domain.Models;

namespace Infrastructure.Entities;

public class RoleEntity
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public IEnumerable<UserRoleEntity>? UserRoles { get; set; }

    public Role ToDomainModel()
    {
        return new Role { Id = Id, Name = Name };
    }

    public static RoleEntity FromDomainModel(Role role)
    {
        return new RoleEntity { Id = role.Id, Name = role.Name };
    }
}
