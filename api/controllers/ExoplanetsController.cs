using Microsoft.AspNetCore.Mvc;
using Orbital.Api.Models.DTOs;
using Orbital.Api.Results;
using Orbital.Api.Services;

namespace Orbital.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ExoplanetsController(IExoplanetService service) : ControllerBase
{
    [HttpGet]
    public Task<Result<IEnumerable<ExoplanetDto>>> GetAll(
        [FromQuery] string? discoveryMethod = null,
        [FromQuery] int? minYear = null,
        [FromQuery] int? maxYear = null) =>
        service.GetAll(discoveryMethod, minYear, maxYear);
}
