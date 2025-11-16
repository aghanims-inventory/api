using System.Net;
using AghanimsInventoryApi.Data;
using AghanimsInventoryApi.Data.Entities;
using AghanimsInventoryApi.Models.V1.RequestModels;
using AghanimsInventoryApi.Models.V1.ResponseModels;
using AghanimsInventoryApi.Models.V1.ResponseModels.Common;
using Microsoft.EntityFrameworkCore;

namespace AghanimsInventoryApi.Services;

public class HeroV1Service
{
    private readonly ILogger<HeroV1Service> _logger;
    private readonly AghanimsInventoryDbContext _dbContext;

    public HeroV1Service(
        ILogger<HeroV1Service> logger,
        AghanimsInventoryDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task<ApiResponse> QueryHeroes(QueryHeroRequest request, CancellationToken cancellationToken)
    {
        IQueryable<Hero> query = _dbContext.Heroes.AsNoTracking();

        if (request.AttributeId.HasValue)
        {
            query = query.Where(x => x.AttributeId == request.AttributeId);
        }

        if (request.AttackTypeId.HasValue)
        {
            query = query.Where(x => x.AttackTypeId == request.AttackTypeId);
        }

        if (request.Complexity.HasValue)
        {
            query = query.Where(x => x.Complexity == request.Complexity);
        }

        var response = await query.Select(x => new QueryHeroResponse()
        {
            Id = x.Id,
            Name = x.Name,
            DisplayName = x.DisplayName,
            ImageUrl = x.ImageUrl
        }).ToListAsync(cancellationToken);

        return ApiResponse.Successful(HttpStatusCode.OK, response);
    }
}
