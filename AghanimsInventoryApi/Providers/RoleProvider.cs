using AghanimsInventoryApi.Constants;
using AghanimsInventoryApi.Data;
using AghanimsInventoryApi.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace AghanimsInventoryApi.Providers;

public class RoleProvider
{
    private readonly ILogger<RoleProvider> _logger;
    private readonly IMemoryCache _memoryCache;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    public RoleProvider(
        ILogger<RoleProvider> logger,
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
            _logger.LogInformation("{ProviderName} has started.", nameof(RoleProvider));

            using var serviceScope = _serviceScopeFactory.CreateScope();

            AghanimsInventoryDbContext dbContext = serviceScope.ServiceProvider.GetRequiredService<AghanimsInventoryDbContext>();

            List<Role> roles = await dbContext.Roles
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            _memoryCache.Set(CacheKeys.RoleCache, roles);

            _logger.LogInformation("{ProviderName} has completed. Cached {RoleCount} roles.", nameof(RoleProvider), roles.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while initializing the {ProviderName}.", nameof(RoleProvider));
        }
        finally
        {
            _semaphore.Release();
        }
    }
}