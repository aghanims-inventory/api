using AghanimsInventoryApi.Constants;
using AghanimsInventoryApi.Data;
using AghanimsInventoryApi.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace AghanimsInventoryApi.Providers;

public class HeroStatProvider
{
    private readonly ILogger<HeroStatProvider> _logger;
    private readonly IMemoryCache _memoryCache;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    public HeroStatProvider(
        ILogger<HeroStatProvider> logger,
        IMemoryCache memoryCache,
        IServiceScopeFactory serviceScopeFactory)
    {
        _logger = logger;
        _memoryCache = memoryCache;
        _serviceScopeFactory = serviceScopeFactory;
    }

    public async Task<List<HeroStat>> GetHeroStats(CancellationToken cancellationToken)
    {
        _memoryCache.TryGetValue(CacheKeys.HeroStatCache, out List<HeroStat>? heroStats);

        if (heroStats is null)
        {
            _logger.LogInformation("Hero stats not found in cache. Initializing cache.");

            await InitializeCache(cancellationToken);

            return _memoryCache.Get<List<HeroStat>>(CacheKeys.HeroStatCache) ?? new List<HeroStat>();
        }

        return heroStats;
    }

    public async Task InitializeCache(CancellationToken cancellationToken)
    {
        await _semaphore.WaitAsync(cancellationToken);

        try
        {
            _logger.LogInformation("{ProviderName} has started.", nameof(HeroStatProvider));

            using var serviceScope = _serviceScopeFactory.CreateScope();

            AghanimsInventoryDbContext dbContext = serviceScope.ServiceProvider.GetRequiredService<AghanimsInventoryDbContext>();

            List<HeroStat> heroStats = await dbContext.HeroStats
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            _memoryCache.Set(CacheKeys.HeroStatCache, heroStats);

            _logger.LogInformation("{ProviderName} has completed. Cached {HeroStatCount} hero stats.", nameof(HeroStatProvider), heroStats.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while initializing the {ProviderName}.", nameof(HeroStatProvider));
        }
        finally
        {
            _semaphore.Release();
        }
    }
}