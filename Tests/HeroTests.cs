using System.Net;
using AghanimsInventoryApi.Constants;
using AghanimsInventoryApi.Data;
using AghanimsInventoryApi.Data.Entities;
using AghanimsInventoryApi.Data.Enums;
using AghanimsInventoryApi.Models.V1.RequestModels;
using AghanimsInventoryApi.Models.V1.ResponseModels;
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
    [Trait("Heroes", nameof(_heroService.QueryHeroes))]
    public async Task QueryHeroes_WhenCacheIsEmpty_ReturnsNotFound()
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
    [Trait("Heroes", nameof(_heroService.QueryHeroes))]
    public async Task QueryHeroes_WhenCacheHasOneHero_ReturnsSuccess()
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

    [Fact]
    [Trait("Heroes", nameof(_heroService.QueryHeroes))]
    public async Task QueryHeroes_WithFilter_WhenNoMatchingHeroInCache_ReturnsEmpty()
    {
        using var cts = new CancellationTokenSource();

        _memoryCache.Set(CacheKeys.HeroCache, new List<Hero>
        {
            TestValues.TestHero
        });

        var result = await _heroService.QueryHeroes(new QueryHeroRequest()
        {
            AttributeId = (int)AttributeTypes.Universal
        }, cts.Token);

        Assert.True(result.IsSuccessful);
        Assert.Null(result.Error);
        Assert.Empty((List<QueryHeroResponse>)result.Data!);
    }

    [Fact]
    [Trait("Heroes", nameof(_heroService.QueryHeroes))]
    public async Task QueryHeroes_WithFilterAttributeId_WhenMatchingHeroInCache_ReturnsData()
    {
        using var cts = new CancellationTokenSource();

        _memoryCache.Set(CacheKeys.HeroCache, new List<Hero>
        {
            TestValues.TestHero
        });

        var result = await _heroService.QueryHeroes(new QueryHeroRequest()
        {
            AttributeId = (int)AttributeTypes.Strength
        }, cts.Token);

        Assert.True(result.IsSuccessful);
        Assert.Null(result.Error);
        Assert.Single((List<QueryHeroResponse>)result.Data!);
    }

    [Fact]
    [Trait("Heroes", nameof(_heroService.QueryHeroes))]
    public async Task QueryHeroes_WithFilterAttackTypeId_WhenMatchingHeroInCache_ReturnsData()
    {
        using var cts = new CancellationTokenSource();

        _memoryCache.Set(CacheKeys.HeroCache, new List<Hero>
        {
            TestValues.TestHero
        });

        var result = await _heroService.QueryHeroes(new QueryHeroRequest()
        {
            AttackTypeId = (int)AttackTypes.Melee
        }, cts.Token);

        Assert.True(result.IsSuccessful);
        Assert.Null(result.Error);
        Assert.Single((List<QueryHeroResponse>)result.Data!);
    }

    [Fact]
    [Trait("Heroes", nameof(_heroService.QueryHeroes))]
    public async Task QueryHeroes_WithFilterComplexity_WhenMatchingHeroInCache_ReturnsData()
    {
        using var cts = new CancellationTokenSource();

        _memoryCache.Set(CacheKeys.HeroCache, new List<Hero>
        {
            TestValues.TestHero
        });

        var result = await _heroService.QueryHeroes(new QueryHeroRequest()
        {
            Complexity = 1
        }, cts.Token);

        Assert.True(result.IsSuccessful);
        Assert.Null(result.Error);
        Assert.Single((List<QueryHeroResponse>)result.Data!);
    }

    [Fact]
    [Trait("Heroes", nameof(_heroService.QueryHeroes))]
    public async Task QueryHeroes_WithFilterWithAllProperties_WhenMatchingHeroInCache_ReturnsData()
    {
        using var cts = new CancellationTokenSource();

        _memoryCache.Set(CacheKeys.HeroCache, new List<Hero>
        {
            TestValues.TestHero
        });

        var result = await _heroService.QueryHeroes(new QueryHeroRequest()
        {
            AttributeId = (int)AttributeTypes.Strength,
            AttackTypeId = (int)AttackTypes.Melee,
            Complexity = 1
        }, cts.Token);

        Assert.True(result.IsSuccessful);
        Assert.Null(result.Error);
        Assert.Single((List<QueryHeroResponse>)result.Data!);
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
            AttributeId = (int)AttributeTypes.Strength,
            AttackTypeId = (int)AttackTypes.Melee,
            Complexity = 1
        };
    }
}
