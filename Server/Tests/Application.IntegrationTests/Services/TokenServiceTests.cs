using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Application.Services;
using Domain;
using Domain.Models;
using IntegrationTests.Helper;
using IntegrationTests.Helper.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Application.IntegrationTests.Services;

public class TokenServiceTests : TestBase
{
    [Test]
    public async Task GenerateJwtToken_ShouldReturnValidToken_WhenUserHasNoRoles()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var tokenService = scope.ServiceProvider.GetRequiredService<ITokenService>();
        var user = new User
        {
            Id = 1,
            Username = "testuser",
            Password = "hash",
            Email = "test@test.com",
            FirstName = "Test",
            LastName = "User",
        };

        // Act
        var token = await tokenService.GenerateJwtToken(user);

        // Assert
        await Assert.That(token).IsNotNullOrEmpty();

        // Verify it's a valid JWT token
        var handler = new JwtSecurityTokenHandler();
        await Assert.That(handler.CanReadToken(token)).IsTrue();

        var jwtToken = handler.ReadJwtToken(token);
        await Assert.That(jwtToken).IsNotNull();
    }

    [Test]
    public async Task GenerateJwtToken_ShouldIncludeUserClaims()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var tokenService = scope.ServiceProvider.GetRequiredService<ITokenService>();
        var user = new User
        {
            Id = 42,
            Username = "johndoe",
            Password = "hash",
            Email = "john@example.com",
            FirstName = "John",
            LastName = "Doe",
        };

        // Act
        var token = await tokenService.GenerateJwtToken(user);

        // Assert
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);

        var claims = jwtToken.Claims.ToList();

        // Verify user ID claim
        var userIdClaim = claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
        await Assert.That(userIdClaim).IsNotNull();
        await Assert.That(userIdClaim!.Value).IsEqualTo("42");

        // Verify username claim
        var usernameClaim = claims.FirstOrDefault(c => c.Type == ClaimTypes.Name);
        await Assert.That(usernameClaim).IsNotNull();
        await Assert.That(usernameClaim!.Value).IsEqualTo("johndoe");

        // Verify email claim
        var emailClaim = claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);
        await Assert.That(emailClaim).IsNotNull();
        await Assert.That(emailClaim!.Value).IsEqualTo("john@example.com");

        // Verify first name claim
        var firstNameClaim = claims.FirstOrDefault(c => c.Type == ClaimTypes.GivenName);
        await Assert.That(firstNameClaim).IsNotNull();
        await Assert.That(firstNameClaim!.Value).IsEqualTo("John");

        // Verify last name claim
        var lastNameClaim = claims.FirstOrDefault(c => c.Type == ClaimTypes.Surname);
        await Assert.That(lastNameClaim).IsNotNull();
        await Assert.That(lastNameClaim!.Value).IsEqualTo("Doe");
    }

    [Test]
    [NotInParallel]
    public async Task GenerateJwtToken_ShouldIncludeRoleClaims_WhenUserHasRoles()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var tokenService = scope.ServiceProvider.GetRequiredService<ITokenService>();

        var user = GetTestUser();
        using var dbContext = CreateDbContext();
        var userBuilder = new UserBuilder(dbContext);
        var userData = userBuilder.WithUser(user).Build();
        var userEntity = userData.Users[0];

        var testUser = new User
        {
            Id = userEntity.Id,
            Username = userEntity.Username,
            Password = userEntity.Password,
            Email = userEntity.Email,
        };

        // Act
        var token = await tokenService.GenerateJwtToken(testUser);

        // Assert
        await Assert.That(token).IsNotNullOrEmpty();

        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);

        var roleClaims = jwtToken.Claims.Where(c => c.Type == ClaimTypes.Role).ToList();
        await Assert.That(roleClaims).Count().IsEqualTo(1);
        await Assert.That(roleClaims.First().Value).IsEqualTo(Constants.Roles.User);
    }

    [Test]
    public async Task GenerateJwtToken_ShouldIncludeIssuerAndAudience()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var tokenService = scope.ServiceProvider.GetRequiredService<ITokenService>();
        var user = new User
        {
            Id = 1,
            Username = "testuser",
            Password = "hash",
            Email = "test@test.com",
        };

        // Act
        var token = await tokenService.GenerateJwtToken(user);

        // Assert
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);

        // Check that issuer and audience are set (from appsettings)
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
        var tokenService = scope.ServiceProvider.GetRequiredService<ITokenService>();
        var user = new User
        {
            Id = 1,
            Username = "testuser",
            Password = "hash",
            Email = "test@test.com",
        };

        var beforeGeneration = DateTime.UtcNow;

        // Act
        var token = await tokenService.GenerateJwtToken(user);

        // Assert
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);

        // Token should have an expiration time
        await Assert.That(jwtToken.ValidTo).IsGreaterThan(beforeGeneration);

        // Based on TokenService implementation, token expires in 15 minutes
        var expectedExpiration = beforeGeneration.AddMinutes(15);
        var tolerance = TimeSpan.FromMinutes(1); // Allow 1 minute tolerance

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
        var tokenService = scope.ServiceProvider.GetRequiredService<ITokenService>();
        var user = new User
        {
            Id = 1,
            Username = "testuser",
            Password = "hash",
            Email = null,
            FirstName = null,
            LastName = null,
        };

        // Act
        var token = await tokenService.GenerateJwtToken(user);

        // Assert
        await Assert.That(token).IsNotNullOrEmpty();

        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);

        var claims = jwtToken.Claims.ToList();

        // Email claim should exist but be empty
        var emailClaim = claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);
        await Assert.That(emailClaim).IsNotNull();
        await Assert.That(emailClaim!.Value).IsEqualTo(string.Empty);

        // First name claim should exist but be empty
        var firstNameClaim = claims.FirstOrDefault(c => c.Type == ClaimTypes.GivenName);
        await Assert.That(firstNameClaim).IsNotNull();
        await Assert.That(firstNameClaim!.Value).IsEqualTo(string.Empty);

        // Last name claim should exist but be empty
        var lastNameClaim = claims.FirstOrDefault(c => c.Type == ClaimTypes.Surname);
        await Assert.That(lastNameClaim).IsNotNull();
        await Assert.That(lastNameClaim!.Value).IsEqualTo(string.Empty);
    }
}
