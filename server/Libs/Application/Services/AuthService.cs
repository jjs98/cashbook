using System.Security.Claims;
using Domain.Models;
using Domain.Utilities;
using FastEndpoints.Security;
using Infrastructure.Repositories.Interfaces;
using InterfaceGenerator;
using Microsoft.Extensions.Configuration;

namespace Application.Services;

[GenerateAutoInterface]
public class AuthService(
    IUserRoleRepository userRoleRepository,
    IUserRepository userRepository,
    IRoleService roleService,
    IConfiguration configuration
) : IAuthService
{
    public async Task<AuthToken> Login(LoginRequest loginRequest)
    {
        var user = await userRepository.GetByUsername(loginRequest.Username);
        if (user == null || !HashingUtility.VerifyPassword(loginRequest.Password, user.Password))
        {
            throw new UnauthorizedAccessException("Invalid credentials");
        }

        return await GenerateJwtToken(user);
    }

    public async Task<AuthToken> Refresh(User user)
    {
        return await GenerateJwtToken(user);
    }

    private async Task<AuthToken> GenerateJwtToken(User user)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.Username),
            new(ClaimTypes.Email, user.Email ?? string.Empty),
            new(ClaimTypes.GivenName, user.FirstName ?? string.Empty),
            new(ClaimTypes.Surname, user.LastName ?? string.Empty),
        };

        var userRoles = await userRoleRepository.GetByUserId(user.Id);
        var roles = await roleService.GetByIds(userRoles.Select(x => x.RoleId));
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role.Name)));

        var jwtToken = JwtBearer.CreateToken(o =>
        {
            o.SigningKey =
                configuration["Jwt:Key"]
                ?? throw new InvalidOperationException("Jwt:Key not found.");
            o.ExpireAt = DateTime.Now.AddMinutes(15);
            o.Issuer = configuration["Jwt:Issuer"];
            o.Audience = configuration["Jwt:Audience"];
            o.User.Roles.AddRange(roles.Select(r => r.Name));
            o.User.Claims.AddRange(claims);
        });

        return new AuthToken(jwtToken, "");
    }
}
