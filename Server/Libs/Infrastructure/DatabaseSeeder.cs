using Domain;
using Domain.Utilities;
using Infrastructure.Entities;

namespace Infrastructure;

public class DatabaseSeeder(AppDbContext context)
{
    public void Seed()
    {
        VerifyRole(Constants.Roles.Admin);
        VerifyRole(Constants.Roles.User);
        VerifyAdmin();
    }

    public void VerifyRole(string roleName)
    {
        if (!context.Roles.Any(x => x.Name == roleName))
        {
            context.Roles.Add(new RoleEntity { Name = roleName });

            context.SaveChanges();
        }
    }

    public void VerifyAdmin()
    {
        var roles = context
            .Roles.Where(x => x.Name == Constants.Roles.Admin || x.Name == Constants.Roles.User)
            .ToDictionary(x => x.Name);

        if (
            !roles.TryGetValue(Constants.Roles.Admin, out var adminRole)
            || !roles.TryGetValue(Constants.Roles.User, out var userRole)
        )
        {
            throw new Exception(
                $"{Constants.Roles.Admin} or {Constants.Roles.User} role not found in the database."
            );
        }

        var adminUser = context.Users.FirstOrDefault(x =>
            x.UserRoles != null
            && x.UserRoles.Any(ur => ur.RoleId == adminRole.Id)
            && x.UserRoles.Any(ur => ur.RoleId == userRole.Id)
        );
        if (adminUser is null)
        {
            var user = new UserEntity
            {
                Username = "admin",
                Password = HashingUtility.HashPassword("admin"),
                UserRoles =
                [
                    new UserRoleEntity { RoleId = adminRole.Id },
                    new UserRoleEntity { RoleId = userRole.Id },
                ],
            };
            context.Users.Add(user);
            context.SaveChanges();
        }
    }
}
