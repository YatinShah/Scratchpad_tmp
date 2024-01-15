
using System.Diagnostics;

using phone2Name;

namespace phone2Name
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var env = builder.Environment.EnvironmentName;
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", false)
                .AddJsonFile($"appsettings.{env}.json", true)
                .AddEnvironmentVariables()
                .Build();

            builder.Configuration.AddConfiguration(config);
            builder.Host.ConfigureLogging(logBuilder =>
            {
                logBuilder
                .AddConfiguration(config)
                .AddConsole()
                .AddDebug();
            });


            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddHealthChecks();
            builder.Services.AddDaprServices();
            builder.Services.AddScoped<Func<NumSystemEnum, INumMapService>>((services) =>
            {
                return (coll =>
                {
                    return coll switch
                    {
                        NumSystemEnum.English => new NumMapService(),
                        NumSystemEnum.Greek => new GreekNumMapService(),
                        _ => new NumMapService()
                    };
                });
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
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
}


public static class ServiceCollectionDaprExtension
{
    public static IServiceCollection AddDaprServices(this IServiceCollection collection)
    {//https://docs.dapr.io/developing-applications/building-blocks/actors/actors-runtime-config/
        collection.AddRouting();
        collection.AddControllers();
        collection.AddActors(options =>
        {
            options.Actors.RegisterActor<NameGenerator>();
        });
        return collection;
    }
}

public static class WebApplicationDaprExtension
{
    public static WebApplication ConfigureDapr(this WebApplication app)
    {
        app.UseRouting();
        app.MapActorsHandlers();
        return app;
    }
}


