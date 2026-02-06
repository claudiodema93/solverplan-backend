using FSH.Modules.Products.Contracts.DTOs;
using Mediator;

namespace FSH.Modules.Products.Contracts.Application.Features.Queries.GetCategoryById;

/// <summary>
/// Query to retrieve a category by its unique identifier.
/// </summary>
public sealed record GetCategoryByIdQuery(int Id) : IQuery<CategoryDto?>;
