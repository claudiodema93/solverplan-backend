using Mediator;

namespace FSH.Modules.Products.Contracts.Features.v1.Commands.DeleteCategory;

/// <summary>
/// Command to delete a category.
/// </summary>
/// <param name="Id">The unique identifier of the category to delete</param>
public sealed record DeleteCategoryCommand(int Id) : ICommand;
