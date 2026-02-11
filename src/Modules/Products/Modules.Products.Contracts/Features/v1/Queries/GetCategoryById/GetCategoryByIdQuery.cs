using FSH.Modules.Products.Contracts.DTOs;
using Mediator;

namespace FSH.Modules.Products.Contracts.Features.v1.Queries.GetCategoryById;

/// <summary>
/// Query to retrieve a category by its unique identifier.
/// </summary>
/// <param name="Id">The unique identifier of the category to retrieve.</param>
public sealed record GetCategoryByIdQuery(int Id) : IQuery<CategoryDto?>;
