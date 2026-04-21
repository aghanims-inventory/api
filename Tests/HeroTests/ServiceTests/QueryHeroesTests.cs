using System.Net;
using AghanimsInventoryApi.Constants;
using AghanimsInventoryApi.Data;
using AghanimsInventoryApi.Data.Entities;
using AghanimsInventoryApi.Data.Enums;
using AghanimsInventoryApi.Models.V1.RequestModels;
using AghanimsInventoryApi.Models.V1.ResponseModels;
using AghanimsInventoryApi.Models.V1.ResponseModels.Common;
using AghanimsInventoryApi.Services;
using ApiTests.Settings;
using MemoryCache.Testing.Moq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;

namespace ApiTests.HeroTests.ServiceTests;

public class QueryHeroesTests
{
    private readonly Mock<ILogger<HeroV1Service>> _logger;
    private readonly AghanimsInventoryDbContext _dbContext;
    private readonly IMemoryCache _memoryCache;
    private readonly HeroV1Service _heroService;

    private const string TraitName = nameof(HeroV1Service);
    private const string TraitValue = $"ServiceTests/{nameof(HeroV1Service.QueryHeroes)}";

    public QueryHeroesTests()
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

        TestDbContext context = new(options);

        context.Database.EnsureCreated();

        return context;
    }

    private HeroV1Service CreateHeroV1Service()
    {
        return new HeroV1Service(_logger.Object, _memoryCache);
    }

    [Fact]
    [Trait(TraitName, $"{TraitValue}")]
    public async Task WhenCacheIsEmpty_ReturnsNotFound()
    {
        using var cts = new CancellationTokenSource();

        ApiResponse<List<QueryHeroResponse>> result = await _heroService.QueryHeroes(new QueryHeroRequest(), cts.Token);

        Assert.False(result.IsSuccessful);
        Assert.Null(result.Data);
        Assert.NotNull(result.Error);
        Assert.Equal((int)HttpStatusCode.NotFound, result.GetStatusCode());
        Assert.Equal(ResourceKeys.HeroesCouldNotBeFound, result.Error.Title);
    }

    [Fact]
    [Trait(TraitName, $"{TraitValue}")]
    public async Task WhenCacheHasAtLeastOneHero_ReturnsSuccess()
    {
        using var cts = new CancellationTokenSource();

        List<Hero> heroes = new()
        {
            CreateHero(1, "alchemist", "Alchemist", (byte)AttributeTypes.Strength, (byte)AttackTypes.Melee, 1),
            CreateHero(2, "bane", "Bane", (byte)AttributeTypes.Universal, (byte)AttackTypes.Ranged, 2)
        };

        _memoryCache.Set(CacheKeys.HeroCache, heroes);

        ApiResponse<List<QueryHeroResponse>> result = await _heroService.QueryHeroes(new QueryHeroRequest(), cts.Token);

        Assert.True(result.IsSuccessful);
        Assert.Null(result.Error);
        Assert.Equal((int)HttpStatusCode.OK, result.GetStatusCode());
        Assert.NotNull(result.Data);
        Assert.Equal(2, result.Data.Count);
    }

    [Fact]
    [Trait(TraitName, $"{TraitValue}")]
    public async Task WithFilter_WhenNoMatchingHeroInCache_ReturnsEmpty()
    {
        using var cts = new CancellationTokenSource();

        List<Hero> heroes = new()
        {
            CreateHero(1, "alchemist", "Alchemist", (byte)AttributeTypes.Strength, (byte)AttackTypes.Melee, 1),
            CreateHero(2, "bane", "Bane", (byte)AttributeTypes.Universal, (byte)AttackTypes.Ranged, 2)
        };

        _memoryCache.Set(CacheKeys.HeroCache, heroes);

        ApiResponse<List<QueryHeroResponse>> result = await _heroService.QueryHeroes(new QueryHeroRequest()
        {
            AttributeId = (int)AttributeTypes.Intelligence
        }, cts.Token);

        Assert.True(result.IsSuccessful);
        Assert.Null(result.Error);
        Assert.Equal((int)HttpStatusCode.OK, result.GetStatusCode());
        Assert.NotNull(result.Data);
        Assert.Empty(result.Data);
    }

    [Fact]
    [Trait(TraitName, $"{TraitValue}")]
    public async Task WithFilterAttributeId_WhenMatchingHeroInCache_ReturnsData()
    {
        using var cts = new CancellationTokenSource();

        List<Hero> heroes = new()
        {
            CreateHero(1, "alchemist", "Alchemist", (byte)AttributeTypes.Strength, (byte)AttackTypes.Melee, 1),
            CreateHero(2, "bane", "Bane", (byte)AttributeTypes.Universal, (byte)AttackTypes.Ranged, 2)
        };

        _memoryCache.Set(CacheKeys.HeroCache, heroes);

        ApiResponse<List<QueryHeroResponse>> result = await _heroService.QueryHeroes(new QueryHeroRequest()
        {
            AttributeId = (int)AttributeTypes.Strength
        }, cts.Token);

        Assert.True(result.IsSuccessful);
        Assert.Null(result.Error);
        Assert.Equal((int)HttpStatusCode.OK, result.GetStatusCode());
        Assert.NotNull(result.Data);
        Assert.Single(result.Data);
    }

    [Fact]
    [Trait(TraitName, $"{TraitValue}")]
    public async Task WithFilterAttackTypeId_WhenMatchingHeroInCache_ReturnsData()
    {
        using var cts = new CancellationTokenSource();

        List<Hero> heroes = new()
        {
            CreateHero(1, "alchemist", "Alchemist", (byte)AttributeTypes.Strength, (byte)AttackTypes.Melee, 1),
            CreateHero(2, "bane", "Bane", (byte)AttributeTypes.Universal, (byte)AttackTypes.Ranged, 2)
        };

        _memoryCache.Set(CacheKeys.HeroCache, heroes);

        ApiResponse<List<QueryHeroResponse>> result = await _heroService.QueryHeroes(new QueryHeroRequest()
        {
            AttackTypeId = (int)AttackTypes.Melee
        }, cts.Token);

        Assert.True(result.IsSuccessful);
        Assert.Null(result.Error);
        Assert.Equal((int)HttpStatusCode.OK, result.GetStatusCode());
        Assert.NotNull(result.Data);
        Assert.Single(result.Data);
    }

    [Fact]
    [Trait(TraitName, $"{TraitValue}")]
    public async Task WithFilterComplexity_WhenMatchingHeroInCache_ReturnsData()
    {
        using var cts = new CancellationTokenSource();

        List<Hero> heroes = new()
        {
            CreateHero(1, "alchemist", "Alchemist", (byte)AttributeTypes.Strength, (byte)AttackTypes.Melee, 1),
            CreateHero(2, "bane", "Bane", (byte)AttributeTypes.Universal, (byte)AttackTypes.Ranged, 2)
        };

        _memoryCache.Set(CacheKeys.HeroCache, heroes);

        ApiResponse<List<QueryHeroResponse>> result = await _heroService.QueryHeroes(new QueryHeroRequest()
        {
            Complexity = 1
        }, cts.Token);

        Assert.True(result.IsSuccessful);
        Assert.Null(result.Error);
        Assert.Equal((int)HttpStatusCode.OK, result.GetStatusCode());
        Assert.NotNull(result.Data);
        Assert.Single(result.Data);
    }

    [Fact]
    [Trait(TraitName, $"{TraitValue}")]
    public async Task WithFilterWithAllProperties_WhenMatchingHeroInCache_ReturnsData()
    {
        using var cts = new CancellationTokenSource();

        List<Hero> heroes = new()
        {
            CreateHero(1, "alchemist", "Alchemist", (byte)AttributeTypes.Strength, (byte)AttackTypes.Melee, 1),
            CreateHero(2, "bane", "Bane", (byte)AttributeTypes.Universal, (byte)AttackTypes.Ranged, 2)
        };

        _memoryCache.Set(CacheKeys.HeroCache, heroes);

        ApiResponse<List<QueryHeroResponse>> result = await _heroService.QueryHeroes(new QueryHeroRequest()
        {
            AttributeId = (int)AttributeTypes.Strength,
            AttackTypeId = (int)AttackTypes.Melee,
            Complexity = 1
        }, cts.Token);

        Assert.True(result.IsSuccessful);
        Assert.Null(result.Error);
        Assert.Equal((int)HttpStatusCode.OK, result.GetStatusCode());
        Assert.NotNull(result.Data);
        Assert.Single(result.Data);
    }

    private static Hero CreateHero(int id, string name, string displayName, byte attributeId, byte attackTypeId, int complexity, string iconUrl = "", string imageUrl = "")
    {
        return new Hero
        {
            Id = id,
            Name = name,
            DisplayName = displayName,
            IconUrl = iconUrl,
            ImageUrl = imageUrl,
            AttributeId = attributeId,
            AttackTypeId = attackTypeId,
            Complexity = complexity
        };
    }
}
