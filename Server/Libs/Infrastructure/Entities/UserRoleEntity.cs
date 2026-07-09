using Domain.Models;

namespace Infrastructure.Entities;

public class UserRoleEntity
{
    public int RoleId { get; set; }
    public int UserId { get; set; }
    public RoleEntity? Role { get; set; }
    public UserEntity? User { get; set; }

    public UserRole ToDomainModel()
    {
        return new UserRole
        {
            RoleId = RoleId,
            UserId = UserId,
            Role = Role?.ToDomainModel(),
            User = User?.ToDomainModel(),
        };
    }

    public static UserRoleEntity FromDomainModel(UserRole userRole)
    {
        return new UserRoleEntity
        {
            RoleId = userRole.RoleId,
            UserId = userRole.UserId,
            Role = userRole.Role != null ? RoleEntity.FromDomainModel(userRole.Role) : null,
            User = userRole.User != null ? UserEntity.FromDomainModel(userRole.User) : null,
        };
    }
}
