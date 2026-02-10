using Mediator;

namespace FSH.Modules.Products.Contracts.Application.Features.v1.Commands.DeleteBomItem;

/// <summary>
/// Command to delete an existing BOM item.
/// </summary>
/// <param name="Id">The unique identifier of the BOM item to delete.</param>
public sealed record DeleteBomItemCommand(int Id) : ICommand;
