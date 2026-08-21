using Domain.Enums;
using Domain.Models;
using Infrastructure.Repositories.Interfaces;
using InterfaceGenerator;

namespace Application.Services;

[GenerateAutoInterface]
public class HealthService(IHealthRepository healthRepository) : IHealthService
{
    public async Task<HealthStatus> GetDatabaseHealth()
    {
        try
        {
            bool canConnect = await healthRepository.GetHealth();

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
