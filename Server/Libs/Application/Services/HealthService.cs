using Domain.Enums;
using Domain.Models;
using Infrastructure;
using InterfaceGenerator;
using Microsoft.EntityFrameworkCore;

namespace Application.Services;

[GenerateAutoInterface]
public class HealthService(IDbContextFactory<AppDbContext> dbContextFactory) : IHealthService
{
    public async Task<HealthStatus> GetDatabaseHealth()
    {
        try
        {
            await using var dbContext = await dbContextFactory.CreateDbContextAsync();

            using var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(100));

            bool canConnect = await dbContext.Database.CanConnectAsync(cts.Token);

            return new HealthStatus(
                "Database",
                canConnect
                    ? HealthStatusEnum.Healthy.ToString()
                    : HealthStatusEnum.Unhealthy.ToString()
            );
        }
        catch (Exception)
        {
            return new HealthStatus("Database", HealthStatusEnum.Unhealthy.ToString());
        }
    }

    public static HealthStatus GetApiHealth()
    {
        return new HealthStatus("Api", HealthStatusEnum.Healthy.ToString());
    }
}
