namespace Orbital.Api.Models.DTOs;

public record PagedResult<T>(
    IReadOnlyList<T> Items,
    int Page,
    int PageSize,
    int Total,
    IReadOnlyDictionary<string, IReadOnlyList<string>>? FilterMetadata = null);
