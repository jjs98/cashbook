using System.Net;
using System.Net.Http.Json;
using FastEndpoints;
using Presentation.Endpoints.Auth;
using Presentation.IntegrationTests.Builder;

namespace Presentation.IntegrationTests.Endpoints.Auth;

public class LoginTests : TestBase
{
    [Test]
    public async Task Login_ReturnsToken_WhenUserExist()
    {
        // Arrange
        using var client = Factory.CreateClient();
        var user = GetTestUser();
        var dbContext = CreateDbContext();
        var userBuilder = new UserBuilder(dbContext);
        var userData = userBuilder.WithUser(user).Build();

        // Act
        var response = await client.POSTAsync<
            LoginEndpoint,
            LoginEndpointRequest,
            LoginEndpointResponse
        >(new LoginEndpointRequest(user.Username, user.Password));

        // Assert
        await Assert.That(response.Response.StatusCode).IsEqualTo(HttpStatusCode.OK);
        await Assert.That(response.Result.Token).IsNotNullOrEmpty();
    }

    [Test]
    public async Task Login_ReturnsUnauthorized_WhenWrongPasswordIsUsed()
    {
        // Arrange
        using var client = Factory.CreateClient();
        var user = GetTestUser();
        var dbContext = CreateDbContext();
        var userBuilder = new UserBuilder(dbContext);
        var userData = userBuilder.WithUser(user).Build();

        // Act
        var response = await client.POSTAsync<
            LoginEndpoint,
            LoginEndpointRequest,
            LoginEndpointResponse
        >(new LoginEndpointRequest(user.Username, "wrongpassword"));

        // Assert
        await Assert.That(response.Response.StatusCode).IsEqualTo(HttpStatusCode.Unauthorized);
        var message = await response.Response.Content.ReadAsStringAsync();
        await Assert.That(message).IsEqualTo("Invalid credentials");
    }

    [Test]
    public async Task Login_ReturnsBadRequest_WhenValidationFails()
    {
        // Arrange
        using var client = Factory.CreateClient();

        // Act
        var response = await client.POSTAsync<
            LoginEndpoint,
            LoginEndpointRequest,
            LoginEndpointResponse
        >(new LoginEndpointRequest("", ""));

        // Assert
        await Assert.That(response.Response.StatusCode).IsEqualTo(HttpStatusCode.BadRequest);
        var validationErrors = await response.Response.Content.ReadFromJsonAsync<ErrorResponse>();
        await Assert.That(validationErrors?.Errors["username"]).Contains("Username is required");
        await Assert.That(validationErrors?.Errors["password"]).Contains("Password is required");
    }

    [Test]
    public async Task Login_ReturnsUnauthorized_WhenUserDoesNotExist()
    {
        // Arrange
        using var client = Factory.CreateClient();

        // Act
        var response = await client.POSTAsync<
            LoginEndpoint,
            LoginEndpointRequest,
            LoginEndpointResponse
        >(new LoginEndpointRequest("nonexistent", "nonexistent"));

        // Assert
        await Assert.That(response.Response.StatusCode).IsEqualTo(HttpStatusCode.Unauthorized);
        var message = await response.Response.Content.ReadAsStringAsync();
        await Assert.That(message).IsEqualTo("User with name nonexistent not found");
    }
}
