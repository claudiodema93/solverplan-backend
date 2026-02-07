using Mediator;

namespace FSH.Modules.Products.Contracts.Application.Features.v1.Commands.DeleteProduct;

/// <summary>
/// Command to delete a product by ID.
/// </summary>
/// <param name="Id">The unique identifier of the product to delete</param>
public sealed record DeleteProductCommand(int Id) : ICommand;
