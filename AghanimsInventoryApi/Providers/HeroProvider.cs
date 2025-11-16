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
            return (List<Hero>)Enumerable.Empty<Hero>();
        }

        return heroes;
    }

    public async Task InitializeCache()
    {
        await _semaphore.WaitAsync();

        try
        {
            _logger.LogInformation("Hero provider has started.");

            using var serviceScope = _serviceScopeFactory.CreateScope();

            AghanimsInventoryDbContext dbContext = serviceScope.ServiceProvider.GetRequiredService<AghanimsInventoryDbContext>();

            List<Hero> heroes = await dbContext.Heroes.ToListAsync();

            _memoryCache.Set(CacheKeys.HeroCache, heroes);

            _logger.LogInformation("Hero provider has completed.");
        }
        finally
        {
            _semaphore.Release();
        }
    }
}
