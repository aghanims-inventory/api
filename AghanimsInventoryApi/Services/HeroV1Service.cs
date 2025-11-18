using System.Net;
using AghanimsInventoryApi.Constants;
using AghanimsInventoryApi.Data.Entities;
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

        var response = heroes.Select(x => new QueryHeroResponse()
        {
            Id = x.Id,
            Name = x.Name,
            DisplayName = x.DisplayName,
            //IconUrl = x.IconUrl,
            //ImageUrl = x.ImageUrl
        });

        return ApiResponse.Successful(HttpStatusCode.OK, response);
    }
}
