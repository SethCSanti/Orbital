using Microsoft.AspNetCore.Mvc;
using Orbital.Api.Models.External;
using Orbital.Api.Results;
using Orbital.Api.Services;

namespace Orbital.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class IssController(IIssService service) : ControllerBase
{
    [HttpGet("position")]
    public Task<Result<IssPositionUpdate>> GetLatestPosition() => service.GetLatestPosition();
}