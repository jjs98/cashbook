using Application;
using Infrastructure;
using Presentation;

namespace Bootstrap;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.RegisterPresentationModule(builder.Configuration);
        builder.Services.RegisterInfrastructureModule(builder.Configuration);
        builder.Services.RegisterApplicationModule();

        var app = builder.Build();

        app.UsePresentationModule();
        app.Services.UseInfrastructureModule();

        app.Run();
    }
}
