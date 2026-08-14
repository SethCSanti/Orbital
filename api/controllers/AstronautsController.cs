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
    public Task<Result<IEnumerable<AstronautDto>>> GetAll() => service.GetAll();

    [HttpGet("{id:int}")]
    public Task<Result<AstronautDto>> GetById(int id) => service.GetById(id);
}
