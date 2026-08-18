using Microsoft.AspNetCore.Mvc;
using Orbital.Api.Models.DTOs;
using Orbital.Api.Results;
using Orbital.Api.Services;

namespace Orbital.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SpaceStationsController(ISpaceStationService service) : ControllerBase
{
    [HttpGet]
    public Task<Result<IEnumerable<SpaceStationDto>>> GetAll() => service.GetAll();

    [HttpGet("{id:int}")]
    public Task<Result<SpaceStationDto>> GetById(int id) => service.GetById(id);
}