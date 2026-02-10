using FSH.Modules.Products.Contracts.Domain.Enums;
using Mediator;

namespace FSH.Modules.Products.Contracts.Application.Features.v1.Commands.UpdateBomItem;

/// <summary>
/// Command to update an existing BOM item.
/// </summary>
/// <param name="Id">The unique identifier of the BOM item to update.</param>
/// <param name="ChildProductId">The identifier of the child product (component).</param>
/// <param name="Quantity">The quantity of the child product required.</param>
/// <param name="Unit">The unit of measurement for the quantity.</param>
/// <param name="IsManual">Whether this item was entered manually by the user.</param>
/// <param name="Notes">Optional notes about this BOM item.</param>
public sealed record UpdateBomItemCommand(
    int Id,
    int ChildProductId,
    double Quantity,
    UnitOfMeasurement Unit,
    bool IsManual,
    string? Notes) : ICommand;
