using Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class HealthRepository(IDbContextFactory<AppDbContext> contextFactory) : IHealthRepository
{
    public async Task<bool> GetHealth()
    {
        await using var dbContext = await contextFactory.CreateDbContextAsync();

        using var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(100));

        return await dbContext.Database.CanConnectAsync(cts.Token);
    }
}
