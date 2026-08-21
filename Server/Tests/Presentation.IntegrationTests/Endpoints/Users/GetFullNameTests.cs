using System.Net;
using System.Net.Http.Json;
using FastEndpoints;
using IntegrationTests.Helper;
using Presentation.Endpoints.Users;

namespace Presentation.IntegrationTests.Endpoints.Users;

public class GetFullNameTests : TestBase
{
    [Test]
    public async Task GetFullName_ReturnsOK_WhenUserIdIsValid()
    {
        // Arrange
        var user = GetTestUser();
        using var dbContext = CreateDbContext();
        (var client, var userEntity) = await CreateAuthenticatedClientAsync(user, dbContext);

        var request = new GetFullNameRequest(userEntity.Id);

        // Act
        var response = await client.GETAsync<GetFullNameEndpoint, GetFullNameRequest, string>(
            request
        );

        // Assert
        await Assert.That(response.Response.StatusCode).IsEqualTo(HttpStatusCode.OK);
        await Assert.That(response.Result).IsEqualTo(user.Username);
    }

    [Test]
    public async Task GetFullName_ReturnsBadRequest_WhenValidationFails()
    {
        // Arrange
        var user = GetTestUser();
        using var dbContext = CreateDbContext();
        (var client, var userEntity) = await CreateAuthenticatedClientAsync(user, dbContext);

        var request = new GetFullNameRequest(-1);

        // Act
        var response = await client.GETAsync<GetFullNameEndpoint, GetFullNameRequest, string>(
            request
        );

        // Assert
        await Assert.That(response.Response.StatusCode).IsEqualTo(HttpStatusCode.BadRequest);
        var validationErrors = await response.Response.Content.ReadFromJsonAsync<ErrorResponse>();
        await Assert.That(validationErrors?.Errors["id"]).Contains("Id is required");
    }

    [Test]
    public async Task GetFullName_ReturnsForbidden_WhenUserIdDoesNotMatch()
    {
        // Arrange
        var user = GetTestUser();
        using var dbContext = CreateDbContext();
        (var client, var userEntity) = await CreateAuthenticatedClientAsync(user, dbContext);

        var request = new GetFullNameRequest(9999);

        // Act
        var response = await client.GETAsync<GetFullNameEndpoint, GetFullNameRequest, string>(
            request
        );

        // Assert
        await Assert.That(response.Response.StatusCode).IsEqualTo(HttpStatusCode.Forbidden);
    }

    [Test]
    public async Task GetUserName_ReturnsUnauthorized_WhenNoBearerToken()
    {
        // Arrange
        using var client = Factory.CreateClient();
        var request = new GetFullNameRequest(1);

        // Act
        var response = await client.GETAsync<GetFullNameEndpoint, GetFullNameRequest, string>(
            request
        );

        // Assert
        await Assert.That(response.Response.StatusCode).IsEqualTo(HttpStatusCode.Unauthorized);
    }
}
