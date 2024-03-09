using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("api/diagnostics")]
public class DiagnosticsController : ControllerBase
{
    [HttpGet]
    [Route("ping")]
    public ActionResult<string> Ping()
    {
        return Ok("Pong");
    }
}