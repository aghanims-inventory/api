using AghanimsInventoryApi.Models.V1.RequestModels;
using AghanimsInventoryApi.Models.V1.ResponseModels;
using AghanimsInventoryApi.Models.V1.ResponseModels.Common;
using AghanimsInventoryApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace AghanimsInventoryApi.Controllers.V1;

[ApiController]
[ApiVersion("1.0")]
[Route("v{version:apiVersion}/heroes")]
[Tags("Heroes")]
public class HeroV1Controller : ControllerBase
{
    private readonly HeroV1Service _heroV1Service;

    public HeroV1Controller(HeroV1Service heroV1Service)
    {
        _heroV1Service = heroV1Service;
    }

    [HttpGet("filters")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<GetHeroPageFilterResponse>))]
    public async Task<IActionResult> PageFilters(CancellationToken cancellationToken)
    {
        ApiResponse<GetHeroPageFilterResponse> response = await _heroV1Service.GetPageFilters(cancellationToken);

        return StatusCode(response.GetStatusCode(), response);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<GetHeroResponse>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(NoData))]
    public async Task<IActionResult> GetHero([FromRoute] int id, CancellationToken cancellationToken)
    {
        ApiResponse<GetHeroResponse> response = await _heroV1Service.GetHero(id, cancellationToken);

        return StatusCode(response.GetStatusCode(), response);
    }

    [HttpGet("{name}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<GetHeroResponse>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(NoData))]
    public async Task<IActionResult> GetHero([FromRoute] string name, CancellationToken cancellationToken)
    {
        ApiResponse<GetHeroResponse> response = await _heroV1Service.GetHero(name, cancellationToken);
        
        return StatusCode(response.GetStatusCode(), response);
    }

    [HttpGet("")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<QueryHeroResponse>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(NoData))]
    public async Task<IActionResult> QueryHeroes([FromQuery] QueryHeroRequest request, CancellationToken cancellationToken)
    {
        ApiResponse<List<QueryHeroResponse>> response = await _heroV1Service.QueryHeroes(request, cancellationToken);

        return StatusCode(response.GetStatusCode(), response);
    }
}
