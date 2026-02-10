using FSH.Modules.Products.Contracts.DTOs;
using Mediator;

namespace FSH.Modules.Products.Contracts.Application.Features.v1.Queries.GetBomItemById;

/// <summary>
/// Query to retrieve a BOM item by its unique identifier.
/// </summary>
/// <param name="Id">The unique identifier of the BOM item.</param>
public sealed record GetBomItemByIdQuery(int Id) : IQuery<BomItemDto?>;
