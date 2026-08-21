using System.Security.Claims;
using Domain.Models;
using FastEndpoints.Security;
using Infrastructure.Repositories.Interfaces;
using InterfaceGenerator;
using Microsoft.Extensions.Configuration;

namespace Application.Services;

[GenerateAutoInterface]
public class TokenService(
    IUserRoleRepository userRoleRepository,
    IRoleService roleService,
    IConfiguration configuration
) : ITokenService
{
    private const int _tokenExpirationMinutes = 15;

    public async Task<string> GenerateJwtToken(User user)
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

        return JwtBearer.CreateToken(o =>
        {
            o.SigningKey =
                configuration["Jwt:Key"]
                ?? throw new InvalidOperationException("Jwt:Key not found.");
            o.ExpireAt = DateTime.Now.AddMinutes(_tokenExpirationMinutes);
            o.Issuer = configuration["Jwt:Issuer"];
            o.Audience = configuration["Jwt:Audience"];
            o.User.Roles.AddRange(roles.Select(r => r.Name));
            o.User.Claims.AddRange(claims);
        });
    }
}
