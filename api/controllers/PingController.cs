using Microsoft.AspNetCore.Mvc;
namespace Orbital.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PingController : ControllerBase
{
    [HttpGet]
    public IActionResult Get() => Ok("Orbital API is alive");
}