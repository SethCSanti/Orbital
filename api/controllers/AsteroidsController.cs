using Microsoft.AspNetCore.Mvc;
using Orbital.Api.Models.DTOs;
using Orbital.Api.Results;
using Orbital.Api.Services;

namespace Orbital.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AsteroidsController(IAsteroidService service) : ControllerBase
{
    [HttpGet]
    public Task<Result<IEnumerable<AsteroidDto>>> GetFeed() => service.GetFeed();
}
