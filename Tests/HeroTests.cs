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

        TestDbContext context = new(options);

        context.Database.EnsureCreated();

        return context;
    }

    private HeroV1Service CreateHeroV1Service()
    {
        return new HeroV1Service(_logger.Object, _memoryCache);
    }

    public class ValidationTests : HeroTests
    {

    }

    public class ServiceTests : HeroTests
    {
        [Fact]
        [Trait($"{nameof(HeroV1Service)}", $"{nameof(ServiceTests)}/{nameof(HeroV1Service.QueryHeroes)}")]
        public async Task QueryHeroes_WhenCacheIsEmpty_ReturnsNotFound()
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
        [Trait($"{nameof(HeroV1Service)}", $"{nameof(ServiceTests)}/{nameof(HeroV1Service.QueryHeroes)}")]
        public async Task QueryHeroes_WhenCacheHasAtLeastOneHero_ReturnsSuccess()
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
        [Trait($"{nameof(HeroV1Service)}", $"{nameof(ServiceTests)}/{nameof(HeroV1Service.QueryHeroes)}")]
        public async Task QueryHeroes_WithFilter_WhenNoMatchingHeroInCache_ReturnsEmpty()
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
        [Trait($"{nameof(HeroV1Service)}", $"{nameof(ServiceTests)}/{nameof(HeroV1Service.QueryHeroes)}")]
        public async Task QueryHeroes_WithFilterAttributeId_WhenMatchingHeroInCache_ReturnsData()
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
        [Trait($"{nameof(HeroV1Service)}", $"{nameof(ServiceTests)}/{nameof(HeroV1Service.QueryHeroes)}")]
        public async Task QueryHeroes_WithFilterAttackTypeId_WhenMatchingHeroInCache_ReturnsData()
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
        [Trait($"{nameof(HeroV1Service)}", $"{nameof(ServiceTests)}/{nameof(HeroV1Service.QueryHeroes)}")]
        public async Task QueryHeroes_WithFilterComplexity_WhenMatchingHeroInCache_ReturnsData()
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
        [Trait($"{nameof(HeroV1Service)}", $"{nameof(ServiceTests)}/{nameof(HeroV1Service.QueryHeroes)}")]
        public async Task QueryHeroes_WithFilterWithAllProperties_WhenMatchingHeroInCache_ReturnsData()
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

        [Fact]
        [Trait($"{nameof(HeroV1Service)}", $"{nameof(ServiceTests)}/{nameof(HeroV1Service.GetHero)}/id")]
        public async Task GetHeroes_WithId_WhenCacheHasNoData_ReturnsHeroesNotFound()
        {
            using var cts = new CancellationTokenSource();

            ApiResponse<GetHeroResponse> result = await _heroService.GetHero(255, cts.Token);

            Assert.False(result.IsSuccessful);
            Assert.NotNull(result.Error);
            Assert.Equal((int)HttpStatusCode.NotFound, result.GetStatusCode());
            Assert.Equal(ResourceKeys.HeroesCouldNotBeFound, result.Error.Title);
        }

        [Fact]
        [Trait($"{nameof(HeroV1Service)}", $"{nameof(ServiceTests)}/{nameof(HeroV1Service.GetHero)}/id")]
        public async Task GetHeroes_WithId_WhenMatchingHeroInCache_ReturnsData()
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
        [Trait($"{nameof(HeroV1Service)}", $"{nameof(ServiceTests)}/{nameof(HeroV1Service.GetHero)}/id")]
        public async Task GetHeroes_WithId_WhenNoMatchingHeroInCache_ReturnsHeroNotFOund()
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
        [Trait($"{nameof(HeroV1Service)}", $"{nameof(ServiceTests)}/{nameof(HeroV1Service.GetHero)}/name")]
        public async Task GetHeroes_WithName_WhenCacheHasNoData_ReturnsHeroesNotFound()
        {
            using var cts = new CancellationTokenSource();

            ApiResponse<GetHeroResponse> result = await _heroService.GetHero("Alchemist", cts.Token);

            Assert.False(result.IsSuccessful);
            Assert.NotNull(result.Error);
            Assert.Equal((int)HttpStatusCode.NotFound, result.GetStatusCode());
            Assert.Equal(ResourceKeys.HeroesCouldNotBeFound, result.Error.Title);
        }

        [Fact]
        [Trait($"{nameof(HeroV1Service)}", $"{nameof(ServiceTests)}/{nameof(HeroV1Service.GetHero)}/name")]
        public async Task GetHeroes_WithName_WhenMatchingHeroInCache_ReturnsData()
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
        [Trait($"{nameof(HeroV1Service)}", $"{nameof(ServiceTests)}/{nameof(HeroV1Service.GetHero)}/name")]
        public async Task GetHeroes_WithName_WhenNoMatchingHeroInCache_ReturnsHeroNotFound()
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
        [Trait($"{nameof(HeroV1Service)}", $"{nameof(ServiceTests)}/{nameof(HeroV1Service.GetHero)}/name")]
        public async Task GetHeroes_WithName_WhenUsingDifferentCase_ReturnsData()
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

        [Fact]
        [Trait($"{nameof(HeroV1Service)}", $"{nameof(ServiceTests)}/{nameof(HeroV1Service.GetPageFilters)}")]
        public async Task GetPageFilters_ReturnsFilters()
        {
            using var cts = new CancellationTokenSource();

            ApiResponse<GetHeroPageFilterResponse> result = await _heroService.GetPageFilters(cts.Token);

            GetHeroPageFilterResponse response = (GetHeroPageFilterResponse)result.Data!;

            Assert.True(result.IsSuccessful);
            Assert.NotNull(response);

            Assert.Equal(response.AttributeTypes.Count, Enum.GetValues<AttributeTypes>().Length);
            Assert.Equal(response.AttackTypes.Count, Enum.GetValues<AttackTypes>().Length);
            Assert.Equal(response.StatTypes.Count, Enum.GetValues<StatTypes>().Length);
        }
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
