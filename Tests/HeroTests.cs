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
        return new HeroV1Service(_logger.Object, _memoryCache);
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

        _memoryCache.Set(CacheKeys.HeroCache, TestValues.TestHeroes);

        var result = await _heroService.QueryHeroes(new QueryHeroRequest(), cts.Token);

        Assert.True(result.IsSuccessful);
        Assert.Null(result.Error);
    }

    [Fact]
    [Trait("Heroes", nameof(_heroService.QueryHeroes))]
    public async Task QueryHeroes_WithFilter_WhenNoMatchingHeroInCache_ReturnsEmpty()
    {
        using var cts = new CancellationTokenSource();

        _memoryCache.Set(CacheKeys.HeroCache, TestValues.TestHeroes);

        var result = await _heroService.QueryHeroes(new QueryHeroRequest()
        {
            AttributeId = (int)AttributeTypes.Intelligence
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

        _memoryCache.Set(CacheKeys.HeroCache, TestValues.TestHeroes);

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

        _memoryCache.Set(CacheKeys.HeroCache, TestValues.TestHeroes);

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

        _memoryCache.Set(CacheKeys.HeroCache, TestValues.TestHeroes);

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

        _memoryCache.Set(CacheKeys.HeroCache, TestValues.TestHeroes);

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

    [Fact]
    [Trait("Heroes", $"{nameof(_heroService.GetHero)}/id")]
    public async Task GetHeroes_WithId_WhenCacheHasNoData_ReturnsHeroesNotFound()
    {
        using var cts = new CancellationTokenSource();

        var result = await _heroService.GetHero(255, cts.Token);

        Assert.False(result.IsSuccessful);
        Assert.NotNull(result.Error);
        Assert.True(result.GetStatusCode() == (int)HttpStatusCode.NotFound);
        Assert.Equal(ResourceKeys.HeroesCouldNotBeFound, result.Error.Title);
    }

    [Fact]
    [Trait("Heroes", $"{nameof(_heroService.GetHero)}/id")]
    public async Task GetHeroes_WithId_WhenMatchingHeroInCache_ReturnsData()
    {
        using var cts = new CancellationTokenSource();

        _memoryCache.Set(CacheKeys.HeroCache, TestValues.TestHeroes);

        var result = await _heroService.GetHero((byte)TestValues.TestHero.Id, cts.Token);

        Assert.True(result.IsSuccessful);
        Assert.Null(result.Error);
        Assert.NotNull((GetHeroResponse)result.Data!);
    }

    [Fact]
    [Trait("Heroes", $"{nameof(_heroService.GetHero)}/id")]
    public async Task GetHeroes_WithId_WhenNoMatchingHeroInCache_ReturnsHeroNotFOund()
    {
        using var cts = new CancellationTokenSource();

        _memoryCache.Set(CacheKeys.HeroCache, TestValues.TestHeroes);

        var result = await _heroService.GetHero(255, cts.Token);

        Assert.False(result.IsSuccessful);
        Assert.NotNull(result.Error);
        Assert.True(result.GetStatusCode() == (int)HttpStatusCode.NotFound);
        Assert.Equal(ResourceKeys.HeroCouldNotBeFound, result.Error.Title);
    }

    [Fact]
    [Trait("Heroes", $"{nameof(_heroService.GetHero)}/name")]
    public async Task GetHeroes_WithName_WhenCacheHasNoData_ReturnsHeroesNotFound()
    {
        using var cts = new CancellationTokenSource();

        var result = await _heroService.GetHero(TestValues.TestHero.Name, cts.Token);

        Assert.False(result.IsSuccessful);
        Assert.NotNull(result.Error);
        Assert.True(result.GetStatusCode() == (int)HttpStatusCode.NotFound);
        Assert.Equal(ResourceKeys.HeroesCouldNotBeFound, result.Error.Title);
    }

    [Fact]
    [Trait("Heroes", $"{nameof(_heroService.GetHero)}/name")]
    public async Task GetHeroes_WithName_WhenMatchingHeroInCache_ReturnsData()
    {
        using var cts = new CancellationTokenSource();

        _memoryCache.Set(CacheKeys.HeroCache, TestValues.TestHeroes);

        var result = await _heroService.GetHero((byte)TestValues.TestHero.Id, cts.Token);

        Assert.True(result.IsSuccessful);
        Assert.Null(result.Error);
        Assert.NotNull((GetHeroResponse)result.Data!);
    }

    [Fact]
    [Trait("Heroes", $"{nameof(_heroService.GetHero)}/name")]
    public async Task GetHeroes_WithName_WhenNoMatchingHeroInCache_ReturnsHeroNotFound()
    {
        using var cts = new CancellationTokenSource();

        _memoryCache.Set(CacheKeys.HeroCache, TestValues.TestHeroes);

        var result = await _heroService.GetHero("test1hero", cts.Token);

        Assert.False(result.IsSuccessful);
        Assert.NotNull(result.Error);
        Assert.True(result.GetStatusCode() == (int)HttpStatusCode.NotFound);
        Assert.Equal(ResourceKeys.HeroCouldNotBeFound, result.Error.Title);
    }

    [Fact]
    [Trait("Heroes", $"{nameof(_heroService.GetHero)}/name")]
    public async Task GetHeroes_WithName_WhenUsingDifferentCase_ReturnsData()
    {
        using var cts = new CancellationTokenSource();

        _memoryCache.Set(CacheKeys.HeroCache, TestValues.TestHeroes);

        var result = await _heroService.GetHero(TestValues.TestHero.Name.ToUpper(), cts.Token);

        Assert.True(result.IsSuccessful);
        Assert.Null(result.Error);
        Assert.NotNull((GetHeroResponse)result.Data!);
    }

    [Fact]
    [Trait("Heroes", $"{nameof(_heroService.GetPageFilters)}")]
    public async Task GetPageFilters_ReturnsFilters()
    {
        using var cts = new CancellationTokenSource();

        var result = await _heroService.GetPageFilters(cts.Token);

        GetHeroPageFilterResponse response = (GetHeroPageFilterResponse)result.Data!;

        Assert.True(result.IsSuccessful);
        Assert.NotNull(response);

        Assert.Equal(response.AttributeTypes.Count, Enum.GetValues<AttributeTypes>().Length);
        Assert.Equal(response.AttackTypes.Count, Enum.GetValues<AttackTypes>().Length);
        Assert.Equal(response.StatTypes.Count, Enum.GetValues<StatTypes>().Length);
    }

    private static class TestValues
    {
        public static readonly Hero TestHero = new()
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

        public static readonly Hero TestHero2 = new()
        {
            Id = 2,
            Name = "bane",
            DisplayName = "Bane",
            IconUrl = string.Empty,
            ImageUrl = string.Empty,
            AttributeId = (int)AttributeTypes.Universal,
            AttackTypeId = (int)AttackTypes.Ranged,
            Complexity = 2
        };

        public static readonly List<Hero> TestHeroes = new List<Hero>()
        {
            TestHero,
            TestHero2
        };
    }
}
