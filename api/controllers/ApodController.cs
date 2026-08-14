using Microsoft.AspNetCore.Mvc;
using Orbital.Api.Models.DTOs;
using Orbital.Api.Results;
using Orbital.Api.Services;

namespace Orbital.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ApodController(IApodService service) : ControllerBase
{
    [HttpGet("latest")]
    public Task<Result<ApodEntryDto>> GetLatest() => service.GetLatest();

    [HttpGet("{date}")]
    public Task<Result<ApodEntryDto>> GetByDate(DateOnly date) => service.GetByDate(date);
}
