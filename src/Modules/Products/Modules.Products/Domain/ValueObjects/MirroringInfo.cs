using FSH.Framework.Core.Domain;

namespace FSH.Modules.Products.Domain.ValueObjects;

/// <summary>
/// Value Object representing mirroring information for a product.
/// </summary>
public sealed class MirroringInfo : ValueObject
{
    /// <summary>
    /// Indicates whether this product is mirrored from another product.
    /// </summary>
    public bool IsMirrored { get; private init; }

    /// <summary>
    /// The title of the source product if this product is mirrored.
    /// </summary>
    public string? SourceProductTitle { get; private init; }

    /// <summary>
    /// The revision of the source product if this product is mirrored.
    /// </summary>
    public string? SourceProductRevision { get; private init; }

    /// <summary>
    /// Indicates whether to copy the product data when mirroring.
    /// </summary>
    public bool CopyProduct { get; private init; }

    private MirroringInfo() { }

    private MirroringInfo(
        bool isMirrored,
        string? sourceProductTitle,
        string? sourceProductRevision,
        bool copyProduct)
    {
        IsMirrored = isMirrored;
        SourceProductTitle = sourceProductTitle;
        SourceProductRevision = sourceProductRevision;
        CopyProduct = copyProduct;
    }

    /// <summary>
    /// Creates a new MirroringInfo instance.
    /// </summary>
    public static MirroringInfo Create(
        bool isMirrored = false,
        string? sourceProductTitle = null,
        string? sourceProductRevision = null,
        bool copyProduct = true)
    {
        return new MirroringInfo(
            isMirrored,
            sourceProductTitle,
            sourceProductRevision,
            copyProduct);
    }

    /// <summary>
    /// Creates a default instance representing a non-mirrored product.
    /// </summary>
    public static MirroringInfo Default() => new();

    /// <summary>
    /// Returns true if mirroring is not enabled.
    /// </summary>
    public bool IsEmpty => !IsMirrored;

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return IsMirrored;
        yield return SourceProductTitle ?? string.Empty;
        yield return SourceProductRevision ?? string.Empty;
        yield return CopyProduct;
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (obj.GetType() != GetType()) return false;

        if (obj is MirroringInfo other)
        {
            return IsMirrored && other.IsMirrored &&
                   SourceProductTitle == other.SourceProductTitle &&
                   SourceProductRevision == other.SourceProductRevision && 
                   CopyProduct == other.CopyProduct;
        }

        return false;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(IsMirrored, SourceProductTitle, SourceProductRevision, CopyProduct);
    }
}