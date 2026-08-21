using Bootstrap;
using Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using TUnit.AspNetCore;

namespace IntegrationTests.Helper;

public class WebApiFactory : TestWebApplicationFactory<Program>
{
    [ClassDataSource<TestDatabase>(Shared = SharedType.PerTestSession)]
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Usage",
        "TUnit0043:Property must use `required` keyword",
        Justification = "Is initialized by the test framework"
    )]
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
