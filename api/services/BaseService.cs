using Orbital.Api.Data;

namespace Orbital.Api.Services;

public abstract class BaseService(OrbitalDbContext context)
{
    protected readonly OrbitalDbContext _context = context;
}