using System.Diagnostics;
using DigitsWorker.Models;
using Grpc.Net.Client;
using Dapr.Client;

namespace DigitsWorker;

public class Program
{
    static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
        builder.Configuration.AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true);
        builder.Configuration.AddEnvironmentVariables();
        builder.Services.Configure<PubSubSetting>(builder.Configuration.GetSection("AppCfg:PubSub"));

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddHealthChecks();

        builder.Services.AddDaprServices();

        builder.Services.AddScoped<IDigitService, DigitService>();

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.ConfigureDapr();

        app.UseAuthorization();
        app.MapControllers();
        app.MapHealthChecks("/health");

        app.Run();
    }
}

public static class ServiceCollectionDaprExtension
{
    public static IServiceCollection AddDaprServices(this IServiceCollection collection)
    {
        collection.AddRouting();
        collection.AddControllers()
            ;
        collection.AddDaprClient();
        return collection;
    }
}

public static class WebApplicationDaprExtension
{
    public static WebApplication ConfigureDapr(this WebApplication app)
    {
        app.UseRouting();
        app.UseCloudEvents();
        app.MapSubscribeHandler();
        return app;
    }
}

