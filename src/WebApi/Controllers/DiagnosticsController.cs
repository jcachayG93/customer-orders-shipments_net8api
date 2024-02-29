using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("api/diagnostics")]
public class DiagnosticsController : ControllerBase
{
    [HttpGet]
    [Route("ping")]
    public async Task<ActionResult<string>> Ping()
    {
        return Ok("Pong");
    }
}