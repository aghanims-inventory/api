using AghanimsInventoryApi.Models.V1.RequestModels;
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

    [HttpGet("")]
    public async Task<IActionResult> QueryHeroes([FromQuery] QueryHeroRequest request, CancellationToken cancellationToken)
    {
        var response = await _heroV1Service.QueryHeroes(request, cancellationToken);

        return StatusCode(response.GetStatusCode(), response);
    }
}
