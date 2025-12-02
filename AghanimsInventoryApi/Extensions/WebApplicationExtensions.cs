using AghanimsInventoryApi.Providers;
using Serilog;

namespace AghanimsInventoryApi.Extensions;

public static class WebApplicationExtensions
{
    public static async Task InitializeProviders(this WebApplication app)
    {
        using var serviceScope = app.Services.CreateScope();

        Log.Information("Providers are being initialized.");

        var serviceProvider = serviceScope.ServiceProvider;

        var applicationLifetime = serviceProvider.GetRequiredService<IHostApplicationLifetime>();

        CancellationToken cancellationToken = applicationLifetime.ApplicationStopping;

        var heroProvider = serviceProvider.GetRequiredService<HeroProvider>();

        var heroProviderTask = heroProvider.InitializeCache(cancellationToken);

        var heroAttributeProvider = serviceProvider.GetRequiredService<HeroAttributeProvider>();

        var heroAttributeProviderTask = heroAttributeProvider.InitializeCache(cancellationToken);

        var heroStatProvider = serviceProvider.GetRequiredService<HeroStatProvider>();

        var heroStatProviderTask = heroStatProvider.InitializeCache(cancellationToken);

        var attributeProvider = serviceProvider.GetRequiredService<AttributeProvider>();

        var attributeProviderTask = attributeProvider.InitializeCache(cancellationToken);

        var attackTypeProvider = serviceProvider.GetRequiredService<AttackTypeProvider>();

        var attackTypeProviderTask = attackTypeProvider.InitializeCache(cancellationToken);

        var statTypeProvider = serviceProvider.GetRequiredService<StatTypeProvider>();

        var statTypeProviderTask = statTypeProvider.InitializeCache(cancellationToken);

        var roleProvider = serviceProvider.GetRequiredService<RoleProvider>();

        var roleProviderTask = roleProvider.InitializeCache(cancellationToken);

        var rarityProvider = serviceProvider.GetRequiredService<RarityProvider>();

        var rarityProviderTask = rarityProvider.InitializeCache(cancellationToken);

        await Task.WhenAll(
            heroProviderTask,
            heroAttributeProviderTask,
            heroStatProviderTask,
            attributeProviderTask,
            attackTypeProviderTask,
            statTypeProviderTask,
            roleProviderTask,
            rarityProviderTask
        );

        Log.Information("Providers have been initialized.");
    }
}
