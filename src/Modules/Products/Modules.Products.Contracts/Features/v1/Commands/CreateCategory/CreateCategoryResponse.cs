namespace FSH.Modules.Products.Contracts.Features.v1.Commands.CreateCategory;

/// <summary>
/// Response containing the identifier of the newly created category.
/// </summary>
/// <param name="Id">The unique identifier of the created category.</param>
public sealed record CreateCategoryResponse(int Id);
