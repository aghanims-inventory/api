using Microsoft.AspNetCore.Mvc;

namespace AghanimsInventoryApi.Controllers.V1;

[Route("heroes")]
[ApiController]
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
