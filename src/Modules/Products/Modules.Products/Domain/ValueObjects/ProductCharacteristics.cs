using FSH.Framework.Core.Domain;

namespace FSH.Modules.Products.Domain.ValueObjects;

/// <summary>
/// Value Object representing the characteristics and type flags of a product.
/// </summary>
public sealed class ProductCharacteristics : ValueObject
{
    /// <summary>
    /// Indicates whether the product is an assembly composed of multiple parts.
    /// </summary>
    public bool IsAssembly { get; private init; }

    /// <summary>
    /// Indicates whether the product is job work (outsourced manufacturing).
    /// </summary>
    public bool IsJobWork { get; private init; }

    /// <summary>
    /// Indicates whether the product can be manufactured.
    /// </summary>
    public bool IsManufacturable { get; private init; }

    /// <summary>
    /// Indicates whether the product is for commercial purposes.
    /// </summary>
    public bool IsCommercial { get; private init; }

    /// <summary>
    /// Indicates whether the product can be sold to customers.
    /// </summary>
    public bool IsSellable { get; private init; }

    /// <summary>
    /// Indicates whether the product requires quality checks.
    /// </summary>
    public bool IsQualityCheckRequired { get; private init; }

    private ProductCharacteristics() { }

    private ProductCharacteristics(
        bool isAssembly,
        bool isJobWork,
        bool isManufacturable,
        bool isCommercial,
        bool isSellable,
        bool isQualityCheckRequired)
    {
        IsAssembly = isAssembly;
        IsJobWork = isJobWork;
        IsManufacturable = isManufacturable;
        IsCommercial = isCommercial;
        IsSellable = isSellable;
        IsQualityCheckRequired = isQualityCheckRequired;
    }

    /// <summary>
    /// Creates a new ProductCharacteristics instance.
    /// </summary>
    public static ProductCharacteristics Create(
        bool isAssembly = false,
        bool isJobWork = false,
        bool isManufacturable = false,
        bool isCommercial = false,
        bool isSellable = false,
        bool isQualityCheckRequired = false)
    {
        return new ProductCharacteristics(
            isAssembly,
            isJobWork,
            isManufacturable,
            isCommercial,
            isSellable,
            isQualityCheckRequired);
    }

    /// <summary>
    /// Creates a default instance with all flags set to false.
    /// </summary>
    public static ProductCharacteristics Default() => new();

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return IsAssembly;
        yield return IsJobWork;
        yield return IsManufacturable;
        yield return IsCommercial;
        yield return IsSellable;
        yield return IsQualityCheckRequired;
    }
}