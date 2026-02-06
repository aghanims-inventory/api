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

    private static readonly TimeSpan SemaphoreWaitTimeout = TimeSpan.FromSeconds(3);

    public HeroStatProvider(
        ILogger<HeroStatProvider> logger,
        IMemoryCache memoryCache,
        IServiceScopeFactory serviceScopeFactory)
    {
        _logger = logger;
        _memoryCache = memoryCache;
        _serviceScopeFactory = serviceScopeFactory;
    }

    public async Task InitializeCache(CancellationToken cancellationToken)
    {
        bool isAcquired = false;

        try
        {
            _logger.LogInformation("{ProviderName} has started.", nameof(HeroStatProvider));

            isAcquired = await _semaphore.WaitAsync(SemaphoreWaitTimeout, cancellationToken);

            if (!isAcquired)
            {
                _logger.LogWarning("{ProviderName} is already processing a request.", nameof(HeroStatProvider));

                return;
            }

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
            if (isAcquired)
            {
                _semaphore.Release();
            }
        }
    }
}
