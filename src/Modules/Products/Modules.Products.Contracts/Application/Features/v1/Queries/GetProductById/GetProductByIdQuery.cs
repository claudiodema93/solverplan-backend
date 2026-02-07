using FSH.Modules.Products.Contracts.DTOs;
using Mediator;

namespace FSH.Modules.Products.Contracts.Application.Features.v1.Queries.GetProductById;

/// <summary>
/// Query to retrieve a product by its unique identifier.
/// </summary>
public sealed record GetProductByIdQuery(int Id) : IQuery<ProductDto?>;
