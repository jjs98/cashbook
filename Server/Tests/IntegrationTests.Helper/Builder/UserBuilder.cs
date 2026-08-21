using Infrastructure;
using Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace IntegrationTests.Helper.Builder;

public class UserBuilder(AppDbContext dbContext)
{
    private readonly List<UserEntity> _users = [];
    private readonly List<RoleEntity> _roles = [];
    private readonly List<UserRoleEntity> _userRoles = [];

    public UserData Result()
    {
        return new UserData(_users, _roles, _userRoles);
    }

    public UserData Build()
    {
        try
        {
            dbContext.SaveChanges();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            throw new Exception(
                "Error saving changes to the database. See inner exception for details.",
                ex.InnerException
            );
        }
        return Result();
    }

    public UserBuilder WithUser(UserEntity user) =>
        WithUser(
            user.Username,
            user.Password,
            user.Email,
            user.FirstName,
            user.LastName,
            user.UserRoles?.Where(x => x.Role is not null).Select(ur => ur.Role!.Name).ToArray()
                ?? []
        );

    public UserBuilder WithUser(string username, string password, string role) =>
        WithUser(username, password, null, null, null, [role]);

    public UserBuilder WithUser(
        string username,
        string password,
        string email,
        string firstName,
        string lastName,
        string role
    ) => WithUser(username, password, email, firstName, lastName, [role]);

    public UserBuilder WithUser(
        string username,
        string password,
        string? email,
        string? firstName,
        string? lastName,
        string[] roles
    )
    {
        var user = new UserEntity
        {
            Username = username,
            Email = email,
            FirstName = firstName,
            LastName = lastName,
            Password = BCrypt.Net.BCrypt.HashPassword(password),
        };
        dbContext.Users.Add(user);
        _users.Add(user);

        foreach (var role in roles)
        {
            var roleEntity = GetOrCreateRole(role);
            var userRole = new UserRoleEntity { User = user, Role = roleEntity };
            dbContext.UserRoles.Add(userRole);
            _userRoles.Add(userRole);
        }

        return this;
    }

    private RoleEntity GetOrCreateRole(string roleName)
    {
        // First check the local context cache
        var localRole = dbContext.Roles.Local.FirstOrDefault(r => r.Name == roleName);
        if (localRole != null)
        {
            return localRole;
        }

        // Then check the database
        var existingRole = dbContext.Roles.AsNoTracking().FirstOrDefault(r => r.Name == roleName);
        if (existingRole != null)
        {
            // Attach it to the context
            dbContext.Roles.Attach(existingRole);
            return existingRole;
        }

        // Create new role
        var newRole = new RoleEntity { Name = roleName };
        dbContext.Roles.Add(newRole);
        _roles.Add(newRole);

        try
        {
            dbContext.SaveChanges();
        }
        catch (DbUpdateException)
        {
            // Role was created by another test, detach our entity and get the existing one
            dbContext.Entry(newRole).State = EntityState.Detached;
            _roles.Remove(newRole);
            var roleFromDb = dbContext.Roles.First(r => r.Name == roleName);
            return roleFromDb;
        }

        return newRole;
    }
}

public record UserData(
    List<UserEntity> Users,
    List<RoleEntity> Roles,
    List<UserRoleEntity> UserRoles
);
