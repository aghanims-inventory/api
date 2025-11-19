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

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<GetHeroResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetHero([FromRoute] byte id, CancellationToken cancellationToken)
    {
        ApiResponse response = await _heroV1Service.GetHero(id, cancellationToken);

        return StatusCode(response.GetStatusCode(), response);
    }

    [HttpGet("{name}")]
    [ProducesResponseType(typeof(ApiResponse<GetHeroResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetHero([FromRoute] string name, CancellationToken cancellationToken)
    {
        ApiResponse response = await _heroV1Service.GetHero(name, cancellationToken);
        
        return StatusCode(response.GetStatusCode(), response);
    }

    [HttpGet("")]
    [ProducesResponseType(typeof(ApiResponse<QueryHeroResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> QueryHeroes([FromQuery] QueryHeroRequest request, CancellationToken cancellationToken)
    {
        ApiResponse response = await _heroV1Service.QueryHeroes(request, cancellationToken);

        return StatusCode(response.GetStatusCode(), response);
    }
}
