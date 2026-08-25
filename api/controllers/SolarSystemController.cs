using Microsoft.AspNetCore.Mvc;
using Orbital.Api.Models.DTOs;
using Orbital.Api.Results;
using Orbital.Api.Services;

namespace Orbital.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SolarSystemController(ISolarSystemService service) : ControllerBase
{
    [HttpGet("bodies")]
    public Task<Result<IEnumerable<PlanetPositionDto>>> GetPositions(
        [FromQuery] DateTimeOffset? at = null) =>
        service.GetPositions(at);
}
