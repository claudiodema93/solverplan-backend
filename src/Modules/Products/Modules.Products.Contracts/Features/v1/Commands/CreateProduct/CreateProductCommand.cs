using FSH.Modules.Products.Contracts.Domain.Enums;
using Mediator;

namespace FSH.Modules.Products.Contracts.Features.v1.Commands.CreateProduct;

/// <summary>
/// Command to create a new product with comprehensive details.
/// </summary>
/// <param name="Title">The product title.</param>
/// <param name="Revision">The product revision number.</param>
/// <param name="Description">Optional product description.</param>
/// <param name="Status">The product status (Draft, Active, Archived, etc.).</param>
/// <param name="Private">Indicates whether the product is private.</param>
/// <param name="CustomerId">Optional customer identifier.</param>
/// <param name="Variant">Product variant identifier.</param>
/// <param name="Version">Product version number.</param>
/// <param name="CategoryId">Optional category identifier to associate with the product.</param>
/// <param name="Keywords">Optional keywords for search and categorization.</param>
/// <param name="Language">Optional language code.</param>
/// <param name="Subject">Optional subject or topic.</param>
/// <param name="Notes">Optional internal notes.</param>
/// <param name="HashSha256">Optional SHA-256 hash for verification.</param>
/// <param name="IsLatest">Indicates whether this is the latest version.</param>
/// <param name="Characteristics">Optional product characteristics (assembly, manufacturability, etc.).</param>
/// <param name="Mirroring">Optional mirroring information for product copies.</param>
/// <example>
/// Example of creating a new product:
/// <code>
/// var command = new CreateProductCommand(
///     Title: "Widget Pro 2000",
///     Revision: 1,
///     Description: "High-performance widget for industrial use",
///     Status: ProductState.Active,
///     CategoryId: 5,
///     Keywords: "widget,industrial,performance",
///     IsLatest: true,
///     Characteristics: new ProductCharacteristicsDto(
///         IsManufacturable: true,
///         IsSellable: true
///     )
/// );
/// </code>
/// </example>
public sealed record CreateProductCommand(
    string Title,
    int Revision = 0,
    string? Description = null,
    ProductState Status = ProductState.Draft,
    bool Private = false,
    string? CustomerId = null,
    string Variant = "",
    int Version = 0,
    int? CategoryId = null,
    string? Keywords = null,
    string? Language = null,
    string? Subject = null,
    string? Notes = null,
    string? HashSha256 = null,
    bool IsLatest = false,
    ProductCharacteristicsDto? Characteristics = null,
    MirroringInfoDto? Mirroring = null) : ICommand<CreateProductResponse>;

/// <summary>
/// DTO for product characteristics.
/// </summary>
/// <param name="IsAssembly">Indicates whether the product is an assembly.</param>
/// <param name="IsJobWork">Indicates whether the product is job work.</param>
/// <param name="IsManufacturable">Indicates whether the product can be manufactured.</param>
/// <param name="IsCommercial">Indicates whether the product is commercial.</param>
/// <param name="IsSellable">Indicates whether the product can be sold.</param>
/// <param name="IsQualityCheckRequired">Indicates whether quality check is required.</param>
public sealed record ProductCharacteristicsDto(
    bool IsAssembly = false,
    bool IsJobWork = false,
    bool IsManufacturable = false,
    bool IsCommercial = false,
    bool IsSellable = false,
    bool IsQualityCheckRequired = false);

/// <summary>
/// DTO for mirroring information.
/// </summary>
/// <param name="IsMirrored">Indicates whether the product is mirrored from another source.</param>
/// <param name="SourceProductTitle">Optional title of the source product.</param>
/// <param name="SourceProductRevision">Optional revision of the source product.</param>
/// <param name="CopyProduct">Indicates whether to copy the product data.</param>
public sealed record MirroringInfoDto(
    bool IsMirrored = false,
    string? SourceProductTitle = null,
    string? SourceProductRevision = null,
    bool CopyProduct = false);