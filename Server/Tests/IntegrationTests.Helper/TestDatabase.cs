using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;
using TUnit.Core.Interfaces;

namespace IntegrationTests.Helper;

public class TestDatabase : IAsyncInitializer, IAsyncDisposable
{
    public readonly PostgreSqlContainer DbContainer = new PostgreSqlBuilder("postgres")
        .WithPortBinding(5432, true)
        .Build();

    public async Task InitializeAsync()
    {
        await DbContainer.StartAsync();

        // Run migrations once when the container starts
        var connectionString = DbContainer.GetConnectionString();
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseNpgsql(connectionString);

        await using var context = new AppDbContext(optionsBuilder.Options);
        await context.Database.MigrateAsync();
    }

    public async ValueTask DisposeAsync()
    {
        await DbContainer.DisposeAsync();
        GC.SuppressFinalize(this);
    }
}
