using FSH.Modules.Products.Contracts.Domain.Enums;
using Mediator;

namespace FSH.Modules.Products.Contracts.Application.Features.v1.Commands.UpdateProduct;

/// <summary>
/// Command to update an existing product with comprehensive details.
/// </summary>
public sealed record UpdateProductCommand(
    int Id,
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
    MirroringInfoDto? Mirroring = null) : ICommand;

/// <summary>
/// DTO for product characteristics.
/// </summary>
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
public sealed record MirroringInfoDto(
    bool IsMirrored = false,
    string? SourceProductTitle = null,
    string? SourceProductRevision = null,
    bool CopyProduct = false);
