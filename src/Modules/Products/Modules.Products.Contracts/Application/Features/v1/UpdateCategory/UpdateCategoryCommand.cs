using Mediator;

namespace FSH.Modules.Products.Contracts.Application.Features.Commands.UpdateCategory;

/// <summary>
/// Command to update an existing category.
/// </summary>
public sealed record UpdateCategoryCommand(int Id, string Name) : ICommand;
