using AghanimsInventoryApi.Data;
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

public class GetPageFiltersTests
{
    private readonly Mock<ILogger<HeroV1Service>> _logger;
    private readonly AghanimsInventoryDbContext _dbContext;
    private readonly IMemoryCache _memoryCache;
    private readonly HeroV1Service _heroService;

    private const string TraitName = nameof(HeroV1Service);
    private const string TraitValue = $"ServiceTests/{nameof(HeroV1Service.GetPageFilters)}";

    public GetPageFiltersTests()
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
    public async Task ReturnsFilters()
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
