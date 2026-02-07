using Mediator;

namespace FSH.Modules.Products.Contracts.Application.Features.Commands.DeleteProduct;

/// <summary>
/// Command to delete a product by ID.
/// </summary>
public sealed record DeleteProductCommand(int Id) : ICommand;
