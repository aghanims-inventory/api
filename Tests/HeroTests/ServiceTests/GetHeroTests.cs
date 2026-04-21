using System.Net;
using AghanimsInventoryApi.Constants;
using AghanimsInventoryApi.Data;
using AghanimsInventoryApi.Data.Entities;
using AghanimsInventoryApi.Data.Enums;
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

public class GetHeroTests
{
    private readonly Mock<ILogger<HeroV1Service>> _logger;
    private readonly AghanimsInventoryDbContext _dbContext;
    private readonly IMemoryCache _memoryCache;
    private readonly HeroV1Service _heroService;

    private const string TraitName = nameof(HeroV1Service);
    private const string TraitValue = $"ServiceTests/{nameof(HeroV1Service.GetHero)}";

    public GetHeroTests()
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
    public async Task WithId_WhenCacheHasNoData_ReturnsHeroesNotFound()
    {
        using var cts = new CancellationTokenSource();

        ApiResponse<GetHeroResponse> result = await _heroService.GetHero(255, cts.Token);

        Assert.False(result.IsSuccessful);
        Assert.NotNull(result.Error);
        Assert.Equal((int)HttpStatusCode.NotFound, result.GetStatusCode());
        Assert.Equal(ResourceKeys.HeroesCouldNotBeFound, result.Error.Title);
    }

    [Fact]
    [Trait(TraitName, $"{TraitValue}")]
    public async Task WithId_WhenMatchingHeroInCache_ReturnsData()
    {
        using var cts = new CancellationTokenSource();

        List<Hero> heroes = new()
        {
            CreateHero(1, "alchemist", "Alchemist", (byte)AttributeTypes.Strength, (byte)AttackTypes.Melee, 1),
            CreateHero(2, "bane", "Bane", (byte)AttributeTypes.Universal, (byte)AttackTypes.Ranged, 2)
        };

        _memoryCache.Set(CacheKeys.HeroCache, heroes);

        ApiResponse<GetHeroResponse> result = await _heroService.GetHero(heroes.First().Id, cts.Token);

        Assert.True(result.IsSuccessful);
        Assert.Null(result.Error);
        Assert.Equal((int)HttpStatusCode.OK, result.GetStatusCode());
        Assert.NotNull(result.Data);
    }

    [Fact]
    [Trait(TraitName, $"{TraitValue}")]
    public async Task WithId_WhenNoMatchingHeroInCache_ReturnsHeroNotFOund()
    {
        using var cts = new CancellationTokenSource();

        List<Hero> heroes = new()
        {
            CreateHero(1, "alchemist", "Alchemist", (byte)AttributeTypes.Strength, (byte)AttackTypes.Melee, 1),
            CreateHero(2, "bane", "Bane", (byte)AttributeTypes.Universal, (byte)AttackTypes.Ranged, 2)
        };

        _memoryCache.Set(CacheKeys.HeroCache, heroes);

        ApiResponse<GetHeroResponse> result = await _heroService.GetHero(255, cts.Token);

        Assert.False(result.IsSuccessful);
        Assert.NotNull(result.Error);
        Assert.Equal((int)HttpStatusCode.NotFound, result.GetStatusCode());
        Assert.Equal(ResourceKeys.HeroCouldNotBeFound, result.Error.Title);
    }

    [Fact]
    [Trait(TraitName, $"{TraitValue}")]
    public async Task WithName_WhenCacheHasNoData_ReturnsHeroesNotFound()
    {
        using var cts = new CancellationTokenSource();

        ApiResponse<GetHeroResponse> result = await _heroService.GetHero("Alchemist", cts.Token);

        Assert.False(result.IsSuccessful);
        Assert.NotNull(result.Error);
        Assert.Equal((int)HttpStatusCode.NotFound, result.GetStatusCode());
        Assert.Equal(ResourceKeys.HeroesCouldNotBeFound, result.Error.Title);
    }

    [Fact]
    [Trait(TraitName, $"{TraitValue}")]
    public async Task WithName_WhenMatchingHeroInCache_ReturnsData()
    {
        using var cts = new CancellationTokenSource();

        List<Hero> heroes = new()
        {
            CreateHero(1, "alchemist", "Alchemist", (byte)AttributeTypes.Strength, (byte)AttackTypes.Melee, 1),
            CreateHero(2, "bane", "Bane", (byte)AttributeTypes.Universal, (byte)AttackTypes.Ranged, 2)
        };

        _memoryCache.Set(CacheKeys.HeroCache, heroes);

        ApiResponse<GetHeroResponse> result = await _heroService.GetHero(heroes.First().Name, cts.Token);

        Assert.True(result.IsSuccessful);
        Assert.Null(result.Error);
        Assert.Equal((int)HttpStatusCode.OK, result.GetStatusCode());
        Assert.NotNull(result.Data);
    }

    [Fact]
    [Trait(TraitName, $"{TraitValue}")]
    public async Task WithName_WhenNoMatchingHeroInCache_ReturnsHeroNotFound()
    {
        using var cts = new CancellationTokenSource();

        List<Hero> heroes = new()
        {
            CreateHero(1, "alchemist", "Alchemist", (byte)AttributeTypes.Strength, (byte)AttackTypes.Melee, 1),
            CreateHero(2, "bane", "Bane", (byte)AttributeTypes.Universal, (byte)AttackTypes.Ranged, 2)
        };

        _memoryCache.Set(CacheKeys.HeroCache, heroes);

        ApiResponse<GetHeroResponse> result = await _heroService.GetHero("test1hero", cts.Token);

        Assert.False(result.IsSuccessful);
        Assert.NotNull(result.Error);
        Assert.Equal((int)HttpStatusCode.NotFound, result.GetStatusCode());
        Assert.Equal(ResourceKeys.HeroCouldNotBeFound, result.Error.Title);
    }

    [Fact]
    [Trait(TraitName, $"{TraitValue}")]
    public async Task WithName_WhenUsingDifferentCase_ReturnsData()
    {
        using var cts = new CancellationTokenSource();

        List<Hero> heroes = new()
        {
            CreateHero(1, "alchemist", "Alchemist", (byte)AttributeTypes.Strength, (byte)AttackTypes.Melee, 1),
            CreateHero(2, "bane", "Bane", (byte)AttributeTypes.Universal, (byte)AttackTypes.Ranged, 2)
        };

        _memoryCache.Set(CacheKeys.HeroCache, heroes);

        ApiResponse<GetHeroResponse> result = await _heroService.GetHero(heroes.First().Name.ToUpper(), cts.Token);

        Assert.True(result.IsSuccessful);
        Assert.Null(result.Error);
        Assert.Equal((int)HttpStatusCode.OK, result.GetStatusCode());
        Assert.NotNull(result.Data);
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
