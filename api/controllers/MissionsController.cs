using Microsoft.AspNetCore.Mvc;
using Orbital.Api.Models.DTOs;
using Orbital.Api.Results;
using Orbital.Api.Services;

namespace Orbital.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MissionsController(IMissionService service) : ControllerBase
{
    [HttpGet]
    public Task<Result<IEnumerable<MissionDto>>> GetAll(
        [FromQuery] string? type = null,
        [FromQuery] string? orbitAbbrev = null) =>
        service.GetAll(type, orbitAbbrev);
}
