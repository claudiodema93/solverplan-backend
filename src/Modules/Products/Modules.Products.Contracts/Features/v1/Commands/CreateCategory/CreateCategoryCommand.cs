using Mediator;

namespace FSH.Modules.Products.Contracts.Features.v1.Commands.CreateCategory;

/// <summary>
/// Command to create a new category.
/// </summary>
/// <param name="Name">The name of the category.</param>
public sealed record CreateCategoryCommand(string Name) : ICommand<int>;
