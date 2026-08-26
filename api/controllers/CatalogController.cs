using Microsoft.AspNetCore.Mvc;
using Orbital.Api.Results;
using Orbital.Api.Services;

namespace Orbital.Api.Controllers;

[ApiController]
[Route("api/catalog")]
public class CatalogController(ICatalogStatusService service) : ControllerBase
{
    [HttpGet("status")]
    public Task<Result<IReadOnlyList<CatalogStatusDto>>> GetStatus() => service.GetAll();
}
