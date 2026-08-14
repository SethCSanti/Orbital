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
    public Task<Result<IEnumerable<RocketDto>>> GetAll() => service.GetAll();

    [HttpGet("{name}")]
    public Task<Result<RocketDto>> GetByName(string name) => service.GetByName(name);

    [HttpPost("compare")]
    public Task<Result<IEnumerable<RocketDto>>> Compare([FromBody] List<string> names) => service.Compare(names);
}
