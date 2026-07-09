using Infrastructure.Repositories;
using Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class Module
{
    public static IServiceCollection RegisterInfrastructureModule(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddDbContextFactory<AppDbContext>(options =>
        {
            var connectionString = configuration.GetConnectionString(nameof(AppDbContext));
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException(
                    $"Connection string '{nameof(AppDbContext)}' not found."
                );
            }
            options.UseNpgsql(connectionString);
        });

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IUserRoleRepository, UserRoleRepository>();

        return services;
    }

    public static IServiceProvider UseInfrastructureModule(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var dbContextFactory = scope.ServiceProvider.GetRequiredService<
            IDbContextFactory<AppDbContext>
        >();
        using var dbContext = dbContextFactory.CreateDbContext();
        dbContext.Database.Migrate();

        var seeder = new DatabaseSeeder(dbContext);
        seeder.Seed();
        return serviceProvider;
    }
}
