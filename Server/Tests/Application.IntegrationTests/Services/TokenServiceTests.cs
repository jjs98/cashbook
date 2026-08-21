using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Application.Services;
using Domain;
using Domain.Models;
using Infrastructure.Repositories.Interfaces;
using IntegrationTests.Helper;
using IntegrationTests.Helper.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using YamlDotNet.Core.Tokens;

namespace Application.IntegrationTests.Services;

public class TokenServiceTests : TestBase
{
    [Test]
    public async Task GenerateJwtToken_ShouldReturnValidToken_WhenUserHasNoRoles()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var tokenService = GetTokenService(scope);
        var user = CreateSimpleUser(firstName: "Test", lastName: "User");

        // Act
        var token = await tokenService.GenerateJwtToken(user);

        // Assert
        await Assert.That(token).IsNotNullOrEmpty();
        var handler = new JwtSecurityTokenHandler();
        await Assert.That(handler.CanReadToken(token)).IsTrue();
        await Assert.That(handler.ReadJwtToken(token)).IsNotNull();
    }

    [Test]
    public async Task GenerateJwtToken_ShouldIncludeUserClaims()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var tokenService = GetTokenService(scope);
        var user = CreateSimpleUser(
            id: 42,
            username: "johndoe",
            email: "john@example.com",
            firstName: "John",
            lastName: "Doe"
        );

        // Act
        var token = await tokenService.GenerateJwtToken(user);

        // Assert
        var jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(token);
        await AssertClaimValue(jwtToken, ClaimTypes.NameIdentifier, "42");
        await AssertClaimValue(jwtToken, ClaimTypes.Name, "johndoe");
        await AssertClaimValue(jwtToken, ClaimTypes.Email, "john@example.com");
        await AssertClaimValue(jwtToken, ClaimTypes.GivenName, "John");
        await AssertClaimValue(jwtToken, ClaimTypes.Surname, "Doe");
    }

    [Test]
    [NotInParallel]
    public async Task GenerateJwtToken_ShouldIncludeRoleClaims_WhenUserHasRoles()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var tokenService = GetTokenService(scope);
        using var dbContext = CreateDbContext();

        var userEntity = new UserBuilder(dbContext).WithUser(GetTestUser()).Build().Users[0];
        var testUser = CreateSimpleUser(
            id: userEntity.Id,
            username: userEntity.Username,
            email: userEntity.Email
        );

        // Act
        var token = await tokenService.GenerateJwtToken(testUser);

        // Assert
        var roleClaims = new JwtSecurityTokenHandler()
            .ReadJwtToken(token)
            .Claims.Where(c => c.Type == ClaimTypes.Role)
            .ToList();
        await Assert.That(roleClaims).Count().IsEqualTo(1);
        await Assert.That(roleClaims.First().Value).IsEqualTo(Constants.Roles.User);
    }

    [Test]
    public async Task GenerateJwtToken_ShouldIncludeIssuerAndAudience()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var tokenService = GetTokenService(scope);
        var user = CreateSimpleUser();

        // Act
        var jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(
            await tokenService.GenerateJwtToken(user)
        );

        // Assert
        await Assert.That(jwtToken.Issuer).IsNotNullOrEmpty();
        var audienceClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "aud");
        await Assert.That(audienceClaim).IsNotNull();
        await Assert.That(audienceClaim!.Value).IsNotNullOrEmpty();
    }

    [Test]
    public async Task GenerateJwtToken_ShouldIncludeExpirationTime()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var tokenService = GetTokenService(scope);
        var user = CreateSimpleUser();
        var beforeGeneration = DateTime.UtcNow;

        // Act
        var jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(
            await tokenService.GenerateJwtToken(user)
        );

        // Assert
        await Assert.That(jwtToken.ValidTo).IsGreaterThan(beforeGeneration);

        var expectedExpiration = beforeGeneration.AddMinutes(15);
        var tolerance = TimeSpan.FromMinutes(1);
        await Assert
            .That(jwtToken.ValidTo)
            .IsGreaterThanOrEqualTo(expectedExpiration.Subtract(tolerance));
        await Assert.That(jwtToken.ValidTo).IsLessThanOrEqualTo(expectedExpiration.Add(tolerance));
    }

    [Test]
    public async Task GenerateJwtToken_ShouldHandleNullOptionalFields()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var tokenService = GetTokenService(scope);
        var user = CreateSimpleUser(email: null, firstName: null, lastName: null);

        // Act
        var jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(
            await tokenService.GenerateJwtToken(user)
        );

        // Assert
        await AssertClaimValue(jwtToken, ClaimTypes.Email, string.Empty);
        await AssertClaimValue(jwtToken, ClaimTypes.GivenName, string.Empty);
        await AssertClaimValue(jwtToken, ClaimTypes.Surname, string.Empty);
    }

    [Test]
    public async Task GenerateJwtToken_ShouldThrowInvalidOperationException_WhenJwtKeyIsNotSet()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var tokenService = new TokenService(
            scope.ServiceProvider.GetRequiredService<IUserRoleRepository>(),
            scope.ServiceProvider.GetRequiredService<IRoleService>(),
            new ConfigurationBuilder().Build()
        );
        var user = CreateSimpleUser();

        // Act & Assert
        await Assert
            .That(async () => await tokenService.GenerateJwtToken(user))
            .Throws<InvalidOperationException>()
            .WithMessage("Jwt:Key not found.");
    }

    private static User CreateSimpleUser(
        int id = 1,
        string username = "testuser",
        string? email = "test@test.com",
        string? firstName = null,
        string? lastName = null
    ) =>
        new()
        {
            Id = id,
            Username = username,
            Password = "hash",
            Email = email,
            FirstName = firstName,
            LastName = lastName,
        };

    private static ITokenService GetTokenService(IServiceScope scope) =>
        scope.ServiceProvider.GetRequiredService<ITokenService>();

    private static async Task AssertClaimValue(
        JwtSecurityToken jwtToken,
        string claimType,
        string expectedValue
    )
    {
        var claim = jwtToken.Claims.FirstOrDefault(c => c.Type == claimType);
        await Assert.That(claim).IsNotNull();
        await Assert.That(claim!.Value).IsEqualTo(expectedValue);
    }
}
