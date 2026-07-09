using Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class Module
{
    public static IServiceCollection RegisterApplicationModule(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IRoleService, RoleService>();
        services.AddScoped<IUserRoleService, UserRoleService>();
        services.AddScoped<IHealthService, HealthService>();

        return services;
    }
}
