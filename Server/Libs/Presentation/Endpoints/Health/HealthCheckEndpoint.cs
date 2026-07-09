using Application.Services;
using Domain.Models;
using FastEndpoints;
using Presentation.Extensions;

namespace Presentation.Endpoints.Health;

public record HealthCheckResponse(HealthStatus[] HealthStatuses, DateTime Timestamp);

public class HealthCheckEndpoint(IHealthService healthService)
    : Endpoint<EmptyRequest, HealthCheckResponse>
{
    public override void Configure()
    {
        Get("/health");
        AllowAnonymous();
        Description(d => d.Produces200<HealthCheckResponse>());
    }

    public override async Task HandleAsync(EmptyRequest req, CancellationToken ct)
    {
        var timestamp = DateTime.UtcNow;
        var statuses = new HealthStatus[]
        {
            await healthService.GetDatabaseHealth(),
            HealthService.GetApiHealth(),
        };
        var response = new HealthCheckResponse(statuses, timestamp);

        await Send.OkAsync(response, ct);
    }
}
