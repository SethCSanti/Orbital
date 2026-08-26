namespace Orbital.Api.Models.Entities;

public class CatalogSyncState
{
    public int Id { get; set; }
    public string Catalog { get; set; } = string.Empty;
    public string Status { get; set; } = "pending";
    public int CurrentPage { get; set; }
    public int PageSize { get; set; } = 100;
    public int? TotalAvailable { get; set; }
    public int RecordsImported { get; set; }
    public DateTimeOffset? LastStartedAt { get; set; }
    public DateTimeOffset? LastCompletedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
    public string? LastError { get; set; }
}
