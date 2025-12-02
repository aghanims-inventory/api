using AghanimsInventoryApi.Constants;
using AghanimsInventoryApi.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace AghanimsInventoryApi.Providers;

public class AttributeProvider
{
    private readonly ILogger<AttributeProvider> _logger;
    private readonly IMemoryCache _memoryCache;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    public AttributeProvider(
        ILogger<AttributeProvider> logger,
        IMemoryCache memoryCache,
        IServiceScopeFactory serviceScopeFactory)
    {
        _logger = logger;
        _memoryCache = memoryCache;
        _serviceScopeFactory = serviceScopeFactory;
    }

    public async Task InitializeCache(CancellationToken cancellationToken)
    {
        await _semaphore.WaitAsync(cancellationToken);

        try
        {
            _logger.LogInformation("{ProviderName} has started.", nameof(AttributeProvider));

            using var serviceScope = _serviceScopeFactory.CreateScope();

            AghanimsInventoryDbContext dbContext = serviceScope.ServiceProvider.GetRequiredService<AghanimsInventoryDbContext>();

            List<Data.Entities.Attribute> attributes = await dbContext.Attributes
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            _memoryCache.Set(CacheKeys.AttributeCache, attributes);

            _logger.LogInformation("{ProviderName} has completed. Cached {AttributeCount} attributes.", nameof(AttributeProvider), attributes.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while initializing the {ProviderName}.", nameof(AttributeProvider));
        }
        finally
        {
            _semaphore.Release();
        }
    }
}