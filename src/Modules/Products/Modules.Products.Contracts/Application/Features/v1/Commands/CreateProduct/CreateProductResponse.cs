namespace FSH.Modules.Products.Contracts.Application.Features.v1.Commands.CreateProduct;

/// <summary>
/// Response containing the identifier of the newly created product.
/// </summary>
/// <param name="Id">The unique identifier of the created product.</param>
public sealed record CreateProductResponse(int Id);
