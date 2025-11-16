using AghanimsInventoryApi.Constants;
using AghanimsInventoryApi.Data;
using AghanimsInventoryApi.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace AghanimsInventoryApi.Providers;

public class HeroProvider
{
    private readonly ILogger<HeroProvider> _logger;
    private readonly IMemoryCache _memoryCache;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    public HeroProvider(
        ILogger<HeroProvider> logger,
        IMemoryCache memoryCache,
        IServiceScopeFactory serviceScopeFactory)
    {
        _logger = logger;
        _memoryCache = memoryCache;
        _serviceScopeFactory = serviceScopeFactory;
    }

    public async Task<List<Hero>> GetHeroes(CancellationToken cancellationToken)
    {
        _memoryCache.TryGetValue(CacheKeys.HeroCache, out List<Hero>? heroes);

        if (heroes is null)
        {
            _logger.LogInformation("Heroes not found in cache. Initializing cache.");

            await InitializeCache(cancellationToken);

            return _memoryCache.Get<List<Hero>>(CacheKeys.HeroCache);
        }

        return heroes;
    }

    public async Task InitializeCache(CancellationToken cancellationToken)
    {
        await _semaphore.WaitAsync(cancellationToken);

        try
        {
            _logger.LogInformation("{ProviderName} has started.", nameof(HeroProvider));

            using var serviceScope = _serviceScopeFactory.CreateScope();

            AghanimsInventoryDbContext dbContext = serviceScope.ServiceProvider.GetRequiredService<AghanimsInventoryDbContext>();

            List<Hero> heroes = await dbContext.Heroes
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            _memoryCache.Set(CacheKeys.HeroCache, heroes);

            _logger.LogInformation("{ProviderName} has completed. Cached {HeroCount} heroes.", nameof(HeroProvider), heroes.Count);
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "An error occurred while initializing the {ProviderName}.", nameof(HeroProvider));
        }
        finally
        {
            _semaphore.Release();
        }
    }
}
