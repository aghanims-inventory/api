using AghanimsInventoryApi.Constants;
using AghanimsInventoryApi.Data;
using AghanimsInventoryApi.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace AghanimsInventoryApi.Providers;

public class StatTypeProvider
{
    private readonly ILogger<StatTypeProvider> _logger;
    private readonly IMemoryCache _memoryCache;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    private static readonly TimeSpan SemaphoreWaitTimeout = TimeSpan.FromSeconds(3);

    public StatTypeProvider(
        ILogger<StatTypeProvider> logger,
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
            _logger.LogInformation("{ProviderName} has started.", nameof(StatTypeProvider));

            isAcquired = await _semaphore.WaitAsync(SemaphoreWaitTimeout, cancellationToken);

            if (!isAcquired)
            {
                _logger.LogWarning("{ProviderName} is already processing a request.", nameof(StatTypeProvider));

                return;
            }

            using var serviceScope = _serviceScopeFactory.CreateScope();

            AghanimsInventoryDbContext dbContext = serviceScope.ServiceProvider.GetRequiredService<AghanimsInventoryDbContext>();

            List<StatType> statTypes = await dbContext.StatTypes
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            _memoryCache.Set(CacheKeys.StatTypeCache, statTypes);

            _logger.LogInformation("{ProviderName} has completed. Cached {StatTypeCount} stat types.", nameof(StatTypeProvider), statTypes.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while initializing the {ProviderName}.", nameof(StatTypeProvider));
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
