using AghanimsInventoryApi.Extensions;
using AghanimsInventoryApi.Providers;
using AghanimsInventoryApi.Services;
using Scalar.AspNetCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Application", "API")
    .Enrich.WithProperty("Host", Environment.MachineName)
    .Enrich.WithProperty("Environment", builder.Environment.EnvironmentName)
    .Enrich.WithProperty("ProjectName", "AghanimsInventory")
    .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Error)
    .MinimumLevel.Override("Microsoft.Hosting.Lifetime", Serilog.Events.LogEventLevel.Information)
    .MinimumLevel.Override("System.Net.Http.HttpClient", Serilog.Events.LogEventLevel.Warning)
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services
    .AddApiVersioningSettings()
    .AddEndpointsApiExplorer()
    .AddDatabaseSettings(builder.Configuration)
    .AddMemoryCache()
    .AddOpenApi();

builder.Services.AddScoped<HeroV1Service>();

builder.Services.AddSingleton<HeroProvider>();

var app = builder.Build();

using (var serviceScope = app.Services.CreateScope())
{
    Log.Information("Providers are being initialized.");

    var applicationLifetime = app.Services.GetRequiredService<IHostApplicationLifetime>();

    CancellationToken cancellationToken = applicationLifetime.ApplicationStopping;

    var heroProvider = serviceScope.ServiceProvider.GetRequiredService<HeroProvider>();

    await heroProvider.InitializeCache(cancellationToken);

    Log.Information("Providers have been initialized.");
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.MapScalarApiReference(options =>
    {
        options
            .WithTitle("Aghanim's Inventory API Documentation")
            .WithTheme(ScalarTheme.Default)
            .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient)
            .AddPreferredSecuritySchemes("Bearer")
            .AddHttpAuthentication("Bearer", bearer =>
            {
                bearer.Token = "your-bearer-token";
            })
            .ShowSidebar = true;
    });
}
else
{
    app.UseHttpsRedirection();
}

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
