using System.Net;
using AghanimsInventoryApi.Constants;
using AghanimsInventoryApi.Data;
using AghanimsInventoryApi.Data.Entities;
using AghanimsInventoryApi.Models.V1.RequestModels;
using AghanimsInventoryApi.Services;
using MemoryCache.Testing.Moq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;

namespace ApiTests;

public class HeroTests
{
    private readonly Mock<ILogger<HeroV1Service>> _logger;
    private readonly AghanimsInventoryDbContext _dbContext;
    private readonly IMemoryCache _memoryCache;
    private readonly HeroV1Service _heroService;

    public HeroTests()
    {
        _logger = new Mock<ILogger<HeroV1Service>>();

        _memoryCache = Create.MockedMemoryCache();

        _dbContext = CreateMockDbContext();

        _heroService = CreateHeroV1Service();
    }

    private AghanimsInventoryDbContext CreateMockDbContext()
    {
        DbContextOptions<AghanimsInventoryDbContext> options = new DbContextOptionsBuilder<AghanimsInventoryDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        AghanimsInventoryDbContext context = new(options);

        context.Database.EnsureCreated();

        return context;
    }

    private HeroV1Service CreateHeroV1Service()
    {
        return new HeroV1Service(_logger.Object,_memoryCache);
    }

    [Fact]
    [Trait("Heroes", "Flow")]
    public async Task EmptyHeroCache_ReturnsNotFound()
    {
        using var cts = new CancellationTokenSource();

        var result = await _heroService.QueryHeroes(new QueryHeroRequest(), cts.Token);

        Assert.False(result.IsSuccessful);
        Assert.Null(result.Data);
        Assert.NotNull(result.Error);

        Assert.False(result.IsSuccessful);
        Assert.NotNull(result.Error);
        Assert.True(result.GetStatusCode() == (int)HttpStatusCode.NotFound);
        Assert.Equal(ResourceKeys.HeroesCouldNotBeFound, result.Error.Title);
    }

    [Fact]
    [Trait("Heroes", "Flow")]
    public async Task CacheWithOneHero_ReturnsSuccessful()
    {
        using var cts = new CancellationTokenSource();

        _memoryCache.Set(CacheKeys.HeroCache, new List<Hero>
        {
            TestValues.TestHero
        });

        var result = await _heroService.QueryHeroes(new QueryHeroRequest(), cts.Token);

        Assert.True(result.IsSuccessful);
        Assert.Null(result.Error);
    }

    private static class TestValues
    {
        public static readonly Hero TestHero = new Hero()
        {
            Id = 1,
            Name = "alchemist",
            DisplayName = "Alchemist",
            IconUrl = string.Empty,
            ImageUrl = string.Empty,
            AttributeId = 1,
            AttackTypeId = 1,
            Complexity = 1
        };
    }
}
