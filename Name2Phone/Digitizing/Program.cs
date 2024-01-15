
using System.Diagnostics;

using Dapr.Client;

using Digitizing.Model;

using FluentValidation;

using Grpc.Net.Client;

using Microsoft.AspNetCore.Authentication;

namespace Digitizing;

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

        builder.Services.Configure<AppCfgSetting>(builder.Configuration.GetSection("AppCfg"));
        builder.Services.AddRouting();
        builder.Services.AddControllers()
            ;
        builder.Services.AddDaprClient();
        builder.Services.AddHealthChecks();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddValidatorsFromAssemblyContaining<Program>();
        builder.Services.AddScoped<ITokenizer, Tokenizer>();

        builder.Services.AddCors(o => o.AddPolicy("MyPolicy", builder =>
       {
           builder.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
       }));

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseCloudEvents();
        app.MapSubscribeHandler();
        app.UseAuthorization();


        app.MapControllers();
        app.MapHealthChecks("/health");

        app.Run();
    }

}
