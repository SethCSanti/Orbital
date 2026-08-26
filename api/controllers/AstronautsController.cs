using Microsoft.AspNetCore.Mvc;
using Orbital.Api.Models.DTOs;
using Orbital.Api.Results;
using Orbital.Api.Services;

namespace Orbital.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AstronautsController(IAstronautService service) : ControllerBase
{
    [HttpGet]
    public Task<Result<PagedResult<AstronautDto>>> GetPage(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 24,
        [FromQuery] string? search = null) => service.GetPage(Math.Max(page, 1), Math.Clamp(pageSize, 1, 100), search);

    [HttpGet("{id:int}")]
    public Task<Result<AstronautDetailDto>> GetById(int id) => service.GetById(id);
}
