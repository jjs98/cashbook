using System.Net;
using Domain.Enums;
using FastEndpoints;
using IntegrationTests.Helper;
using Presentation.Endpoints.Health;

namespace Presentation.IntegrationTests.Endpoints.Health;

public class HealthCheckTests : TestBase
{
    [Test]
    public async Task HealthCheck_ReturnsOK_WhenHealthy()
    {
        // Arrange
        using var client = Factory.CreateClient();
        var datetime = DateTime.UtcNow;

        // Act
        var response = await client.GETAsync<
            HealthCheckEndpoint,
            EmptyRequest,
            HealthCheckResponse
        >(EmptyRequest.Instance);

        // Assert
        await Assert.That(response.Response.StatusCode).IsEqualTo(HttpStatusCode.OK);
        await Assert.That(response.Result.Timestamp).IsAfter(datetime);
        await Assert
            .That(response.Result.HealthStatuses)
            .Any(x => x.ModuleName == "Api" && x.Status == HealthStatusEnum.Healthy.ToString());
        await Assert
            .That(response.Result.HealthStatuses)
            .Any(x =>
                x.ModuleName == "Database" && x.Status == HealthStatusEnum.Healthy.ToString()
            );
    }

    [Test]
    [NotInParallel]
    public async Task HealthCheck_ReturnsOK_WhenDatabaseUnHealthy()
    {
        // Arrange
        using var client = Factory.CreateClient();
        var datetime = DateTime.UtcNow;

        await Database.DbContainer.StopAsync();

        // Act
        var response = await client.GETAsync<
            HealthCheckEndpoint,
            EmptyRequest,
            HealthCheckResponse
        >(EmptyRequest.Instance);

        // Assert
        await Assert.That(response.Response.StatusCode).IsEqualTo(HttpStatusCode.OK);
        await Assert.That(response.Result.Timestamp).IsAfter(datetime);
        await Assert
            .That(response.Result.HealthStatuses)
            .Any(x => x.ModuleName == "Api" && x.Status == HealthStatusEnum.Healthy.ToString());
        await Assert
            .That(response.Result.HealthStatuses)
            .Any(x =>
                x.ModuleName == "Database" && x.Status == HealthStatusEnum.Unhealthy.ToString()
            );
    }
}
