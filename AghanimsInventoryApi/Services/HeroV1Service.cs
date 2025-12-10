using System.Net;
using AghanimsInventoryApi.Constants;
using AghanimsInventoryApi.Data.Entities;
using AghanimsInventoryApi.Data.Enums;
using AghanimsInventoryApi.Models.V1.RequestModels;
using AghanimsInventoryApi.Models.V1.ResponseModels;
using AghanimsInventoryApi.Models.V1.ResponseModels.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace AghanimsInventoryApi.Services;

public class HeroV1Service
{
    private readonly ILogger<HeroV1Service> _logger;
    //private readonly AghanimsInventoryDbContext _dbContext;
    //private readonly HeroProvider _heroProvider;
    private readonly IMemoryCache _memoryCache;

    public HeroV1Service(
        ILogger<HeroV1Service> logger,
        //AghanimsInventoryDbContext dbContext,
        //HeroProvider heroProvider,
        IMemoryCache memoryCache)
    {
        _logger = logger;
        //_dbContext = dbContext;
        //_heroProvider = heroProvider;
        _memoryCache = memoryCache;
    }

    public async Task<ApiResponse> GetPageFilters(CancellationToken cancellationToken)
    {
        GetHeroPageFilterResponse response = new()
        {
            AttributeTypes = Enum.GetValues<AttributeTypes>()
                .Select(x => new GetHeroPageFilterAttributeResponse()
                {
                    Id = (byte)x,
                    Name = x.ToString()
                })
                .ToList(),
            AttackTypes = Enum.GetValues<AttackTypes>()
                .Select(x => new GetHeroPageFilterAttackTypeResponse()
                {
                    Id = (byte)x,
                    Name = x.ToString()
                })
                .ToList(),
            StatTypes = Enum.GetValues<StatTypes>()
                .Select(x => new GetHeroPageFilterStatTypeResponse()
                {
                    Id = (byte)x,
                    Name = x.ToString()
                })
                .ToList()
        };

        return ApiResponse.Successful(HttpStatusCode.OK, response);
    }

    public async Task<ApiResponse> GetHero(byte id, CancellationToken cancellationToken)
    {
        _memoryCache.TryGetValue(CacheKeys.HeroCache, out IEnumerable<Hero>? heroes);

        if (heroes is null || !heroes.Any())
        {
            _logger.LogError("Heroes could not be found.");

            return ApiResponse.Unsuccessful(HttpStatusCode.NotFound, new ProblemDetails()
            {
                Status = StatusCodes.Status404NotFound,
                Title = ResourceKeys.HeroesCouldNotBeFound,
                Detail = ResourceKeys.HeroesCouldNotBeFound
            });
        }

        Hero? hero = heroes.FirstOrDefault(x => x.Id == id);

        if (hero is null)
        {
            _logger.LogError("Hero with id: {HeroId} could not be found.", id);

            return ApiResponse.Unsuccessful(HttpStatusCode.NotFound, new ProblemDetails()
            {
                Status = StatusCodes.Status404NotFound,
                Title = ResourceKeys.HeroCouldNotBeFound,
                Detail = ResourceKeys.HeroCouldNotBeFound
            });
        }

        GetHeroResponse response = new()
        {
            Id = hero.Id,
            Name = hero.Name,
            DisplayName = hero.DisplayName,
            Complexity = hero.Complexity,
            //IconUrl = hero.IconUrl,
            //ImageUrl = hero.ImageUrl,
            AttributeId = hero.AttributeId,
            AttackTypeId = hero.AttackTypeId,
            FormattedAttribute = ((AttributeTypes)hero.AttributeId).ToString(),
            FormattedAttackType = ((AttackTypes)hero.AttackTypeId).ToString()
        };

        return ApiResponse.Successful(HttpStatusCode.OK, response);
    }

    public async Task<ApiResponse> GetHero(string name, CancellationToken cancellationToken)
    {
        _memoryCache.TryGetValue(CacheKeys.HeroCache, out IEnumerable<Hero>? heroes);

        if (heroes is null || !heroes.Any())
        {
            _logger.LogError("Heroes could not be found.");

            return ApiResponse.Unsuccessful(HttpStatusCode.NotFound, new ProblemDetails()
            {
                Status = StatusCodes.Status404NotFound,
                Title = ResourceKeys.HeroesCouldNotBeFound,
                Detail = ResourceKeys.HeroesCouldNotBeFound
            });
        }

        var hero = heroes.FirstOrDefault(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

        if (hero is null)
        {
            _logger.LogError("Hero with name: {HeroName} could not be found.", name);

            return ApiResponse.Unsuccessful(HttpStatusCode.NotFound, new ProblemDetails()
            {
                Status = StatusCodes.Status404NotFound,
                Title = ResourceKeys.HeroCouldNotBeFound,
                Detail = ResourceKeys.HeroCouldNotBeFound
            });
        }

        GetHeroResponse response = new()
        {
            Id = hero.Id,
            Name = hero.Name,
            DisplayName = hero.DisplayName,
            Complexity = hero.Complexity,
            //IconUrl = hero.IconUrl,
            //ImageUrl = hero.ImageUrl,
            AttributeId = hero.AttributeId,
            AttackTypeId = hero.AttackTypeId,
            FormattedAttribute = ((AttributeTypes)hero.AttributeId).ToString(),
            FormattedAttackType = ((AttackTypes)hero.AttackTypeId).ToString()
        };

        return ApiResponse.Successful(HttpStatusCode.OK, response);
    }

    public async Task<ApiResponse> QueryHeroes(QueryHeroRequest request, CancellationToken cancellationToken)
    {
        //IEnumerable<Hero> query = await _heroProvider.GetHeroes(cancellationToken);

        _memoryCache.TryGetValue(CacheKeys.HeroCache, out IEnumerable<Hero>? heroes);

        if (heroes is null || !heroes.Any())
        {
            _logger.LogError("Heroes could not be found.");

            return ApiResponse.Unsuccessful(HttpStatusCode.NotFound, new ProblemDetails()
            {
                Status = StatusCodes.Status404NotFound,
                Title = ResourceKeys.HeroesCouldNotBeFound,
                Detail = ResourceKeys.HeroesCouldNotBeFound
            });
        }

        if (request.AttributeId.HasValue)
        {
            heroes = heroes.Where(x => x.AttributeId == request.AttributeId.Value);
        }

        if (request.AttackTypeId.HasValue)
        {
            heroes = heroes.Where(x => x.AttackTypeId == request.AttackTypeId.Value);
        }

        if (request.Complexity.HasValue)
        {
            heroes = heroes.Where(x => x.Complexity == request.Complexity.Value);
        }

        List<QueryHeroResponse> response = heroes.Select(x => new QueryHeroResponse()
        {
            Id = x.Id,
            Name = x.Name,
            DisplayName = x.DisplayName,
            //IconUrl = x.IconUrl,
            //ImageUrl = x.ImageUrl
            AttributeId = x.AttributeId,
            AttackTypeId = x.AttackTypeId,
            FormattedAttribute = ((AttributeTypes)x.AttributeId).ToString(),
            FormattedAttackType = ((AttackTypes)x.AttackTypeId).ToString()
        }).ToList();

        return ApiResponse.Successful(HttpStatusCode.OK, response);
    }
}
