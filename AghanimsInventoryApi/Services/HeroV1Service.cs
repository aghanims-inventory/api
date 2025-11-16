using System.Net;
using AghanimsInventoryApi.Data;
using AghanimsInventoryApi.Data.Entities;
using AghanimsInventoryApi.Models.V1.RequestModels;
using AghanimsInventoryApi.Models.V1.ResponseModels;
using AghanimsInventoryApi.Models.V1.ResponseModels.Common;
using AghanimsInventoryApi.Providers;
using Microsoft.EntityFrameworkCore;

namespace AghanimsInventoryApi.Services;

public class HeroV1Service
{
    //private readonly ILogger<HeroV1Service> _logger;
    //private readonly AghanimsInventoryDbContext _dbContext;
    private readonly HeroProvider _heroProvider;

    public HeroV1Service(
        //ILogger<HeroV1Service> logger,
        //AghanimsInventoryDbContext dbContext,
        HeroProvider heroProvider)
    {
        //_logger = logger;
        //_dbContext = dbContext;
        _heroProvider = heroProvider;
    }

    public async Task<ApiResponse> QueryHeroes(QueryHeroRequest request, CancellationToken cancellationToken)
    {
        IEnumerable<Hero> query = await _heroProvider.GetHeroes(cancellationToken);

        if (request.AttributeId.HasValue)
        {
            query = query.Where(x => x.AttributeId == request.AttributeId.Value);
        }

        if (request.AttackTypeId.HasValue)
        {
            query = query.Where(x => x.AttackTypeId == request.AttackTypeId.Value);
        }

        if (request.Complexity.HasValue)
        {
            query = query.Where(x => x.Complexity == request.Complexity.Value);
        }

        var response = query.Select(x => new QueryHeroResponse()
        {
            Id = x.Id,
            Name = x.Name,
            DisplayName = x.DisplayName,
            ImageUrl = x.ImageUrl
        });

        return ApiResponse.Successful(HttpStatusCode.OK, response);
    }
}
