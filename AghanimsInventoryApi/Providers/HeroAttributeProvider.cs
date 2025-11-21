using AghanimsInventoryApi.Constants;
using AghanimsInventoryApi.Data;
using AghanimsInventoryApi.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace AghanimsInventoryApi.Providers;

public class HeroAttributeProvider
{
    private readonly ILogger<HeroAttributeProvider> _logger;
    private readonly IMemoryCache _memoryCache;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    public HeroAttributeProvider(
        ILogger<HeroAttributeProvider> logger,
        IMemoryCache memoryCache,
        IServiceScopeFactory serviceScopeFactory)
    {
        _logger = logger;
        _memoryCache = memoryCache;
        _serviceScopeFactory = serviceScopeFactory;
    }

    public async Task<List<HeroAttribute>> GetHeroAttributes(CancellationToken cancellationToken)
    {
        _memoryCache.TryGetValue(CacheKeys.HeroAttributeCache, out List<HeroAttribute>? heroAttributes);

        if (heroAttributes is null)
        {
            _logger.LogInformation("Hero attributes not found in cache. Initializing cache.");

            await InitializeCache(cancellationToken);

            return _memoryCache.Get<List<HeroAttribute>>(CacheKeys.HeroAttributeCache) ?? new List<HeroAttribute>();
        }

        return heroAttributes;
    }

    public async Task InitializeCache(CancellationToken cancellationToken)
    {
        await _semaphore.WaitAsync(cancellationToken);

        try
        {
            _logger.LogInformation("{ProviderName} has started.", nameof(HeroAttributeProvider));

            using var serviceScope = _serviceScopeFactory.CreateScope();

            AghanimsInventoryDbContext dbContext = serviceScope.ServiceProvider.GetRequiredService<AghanimsInventoryDbContext>();

            List<HeroAttribute> heroAttributes = await dbContext.HeroAttributes
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            _memoryCache.Set(CacheKeys.HeroAttributeCache, heroAttributes);

            _logger.LogInformation("{ProviderName} has completed. Cached {HeroAttributeCount} hero attributes.", nameof(HeroAttributeProvider), heroAttributes.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while initializing the {ProviderName}.", nameof(HeroAttributeProvider));
        }
        finally
        {
            _semaphore.Release();
        }
    }
}