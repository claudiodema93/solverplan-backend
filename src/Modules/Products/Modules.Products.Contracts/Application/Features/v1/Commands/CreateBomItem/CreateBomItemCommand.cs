using FSH.Modules.Products.Contracts.Domain.Enums;
using Mediator;

namespace FSH.Modules.Products.Contracts.Application.Features.v1.Commands.CreateBomItem;

/// <summary>
/// Command to create a new BOM (Bill of Materials) item.
/// </summary>
/// <param name="ProductId">The identifier of the parent product.</param>
/// <param name="ChildProductId">The identifier of the child product (component).</param>
/// <param name="Quantity">The quantity of the child product required.</param>
/// <param name="Unit">The unit of measurement for the quantity.</param>
/// <param name="IsManual">Whether this item was entered manually by the user.</param>
/// <param name="Notes">Optional notes about this BOM item.</param>
public sealed record CreateBomItemCommand(
    int ProductId,
    int ChildProductId,
    double Quantity = 1.0,
    UnitOfMeasurement Unit = UnitOfMeasurement.Pcs,
    bool IsManual = false,
    string? Notes = null) : ICommand<int>;
