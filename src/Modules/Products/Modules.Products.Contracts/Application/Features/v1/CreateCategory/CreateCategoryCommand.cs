using Mediator;

namespace FSH.Modules.Products.Contracts.Application.Features.Commands.CreateCategory;

/// <summary>
/// Command to create a new category.
/// </summary>
public sealed record CreateCategoryCommand(string Name) : ICommand<int>;
