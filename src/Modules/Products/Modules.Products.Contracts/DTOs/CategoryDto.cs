namespace FSH.Modules.Products.Contracts.DTOs;

/// <summary>
/// Data transfer object for Category entity.
/// </summary>
public sealed class CategoryDto
{
    /// <summary>
    /// The unique identifier of the category.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// The name of the category.
    /// </summary>
    public string Name { get; set; } = default!;

    /// <summary>
    /// The identifier of the tenant that owns this category.
    /// </summary>
    public string TenantId { get; set; } = default!;

    /// <summary>
    /// The UTC date and time when the category was created.
    /// </summary>
    public DateTimeOffset CreatedOnUtc { get; set; }

    /// <summary>
    /// The identifier of the user who created the category.
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// The UTC date and time when the category was last modified.
    /// </summary>
    public DateTimeOffset? LastModifiedOnUtc { get; set; }

    /// <summary>
    /// The identifier of the user who last modified the category.
    /// </summary>
    public string? LastModifiedBy { get; set; }
}
