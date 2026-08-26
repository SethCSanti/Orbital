using Microsoft.AspNetCore.Mvc;
using Orbital.Api.Models.DTOs;
using Orbital.Api.Results;
using Orbital.Api.Services;

namespace Orbital.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RocketsController(IRocketService service) : ControllerBase
{
    [HttpGet]
    public Task<Result<PagedResult<RocketDto>>> GetPage(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 24,
        [FromQuery] string? search = null) => service.GetPage(Math.Max(page, 1), Math.Clamp(pageSize, 1, 100), search);

    [HttpGet("id/{id:int}")]
    public Task<Result<RocketDetailDto>> GetById(int id) => service.GetById(id);

    [HttpGet("{name}")]
    public Task<Result<RocketDto>> GetByName(string name) => service.GetByName(name);

    [HttpPost("compare")]
    public Task<Result<IEnumerable<RocketDto>>> Compare([FromBody] List<int> ids) => service.Compare(ids);
}
