namespace FSH.Modules.Products.Contracts.DTOs;

/// <summary>
/// Data transfer object for Category entity.
/// </summary>
public sealed class CategoryDto
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
    public string TenantId { get; set; } = default!;
    public DateTimeOffset CreatedOnUtc { get; set; }
    public string? CreatedBy { get; set; }
    public DateTimeOffset? LastModifiedOnUtc { get; set; }
    public string? LastModifiedBy { get; set; }
}
