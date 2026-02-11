using Mediator;

namespace FSH.Modules.Products.Contracts.Features.v1.Commands.UpdateCategory;

/// <summary>
/// Command to update an existing category.
/// </summary>
/// <param name="Id">The unique identifier of the category to update</param>
/// <param name="Name">The new name for the category</param>
public sealed record UpdateCategoryCommand(int Id, string Name) : ICommand;
