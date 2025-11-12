using Microsoft.AspNetCore.Mvc;

namespace AghanimsInventoryApi.Controllers.V1;

[ApiController]
[ApiVersion("1.0")]
[Route("v{version:apiVersion}/heroes")]
[Tags("Heroes")]
public class HeroV1Controller : ControllerBase
{
    public HeroV1Controller()
    {

    }

    [HttpGet("")]
    public async Task<IActionResult> Test()
    {
        return Ok();
    }
}
