using Bogus;
using Bootstrap;
using Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using TUnit.AspNetCore;

namespace Presentation.IntegrationTests;

public class WebApiFactory : TestWebApplicationFactory<Program>
{
    [ClassDataSource<TestDatabase>(Shared = SharedType.PerTestSession)]
    public TestDatabase Database { get; init; } = null!;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<IDbContextFactory<AppDbContext>>();

            services.AddDbContextFactory<AppDbContext>(options =>
            {
                options.UseNpgsql(
                    Database.DbContainer.GetConnectionString(),
                    options =>
                    {
                        options.EnableRetryOnFailure();
                    }
                );
            });
        });
    }
}
