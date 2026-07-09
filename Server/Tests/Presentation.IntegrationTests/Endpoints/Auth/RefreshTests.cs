using System.Net;
using FastEndpoints;
using Presentation.Endpoints.Auth;

namespace Presentation.IntegrationTests.Endpoints.Auth;

public class RefreshTests : TestBase
{
    [Test]
    public async Task Refresh_ReturnsOK_WhenUserExist()
    {
        // Arrange
        var user = GetTestUser();
        using var dbContext = CreateDbContext();
        (var client, var userEntity) = await CreateAuthenticatedClientAsync(user, dbContext);

        // Act
        var response = await client.POSTAsync<
            RefreshEndpoint,
            EmptyRequest,
            RefreshEndpointResponse
        >(new EmptyRequest());

        // Assert
        await Assert.That(response.Response.StatusCode).IsEqualTo(HttpStatusCode.OK);
        await Assert.That(response.Result.Token).IsNotNullOrEmpty();
    }

    [Test]
    public async Task Refresh_ReturnsUnauthorized_WhenNoBearerTokenExist()
    {
        // Arrange
        using var client = Factory.CreateClient();

        // Act
        var response = await client.POSTAsync<
            RefreshEndpoint,
            EmptyRequest,
            RefreshEndpointResponse
        >(new EmptyRequest());

        // Assert
        await Assert.That(response.Response.StatusCode).IsEqualTo(HttpStatusCode.Unauthorized);
    }
}
