using Microsoft.AspNetCore.Mvc;
using Orbital.Api.Models.DTOs;
using Orbital.Api.Results;
using Orbital.Api.Services;

namespace Orbital.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LaunchController(ILaunchService service) : ControllerBase
{
    [HttpGet("upcoming")]
    public Task<Result<IEnumerable<LaunchDto>>> GetUpcoming([FromQuery] string? rocketName = null) =>
        service.GetUpcoming(rocketName);

    [HttpGet("past")]
    public Task<Result<IEnumerable<LaunchDto>>> GetPast([FromQuery] string? rocketName = null) =>
        service.GetPast(rocketName);
}
