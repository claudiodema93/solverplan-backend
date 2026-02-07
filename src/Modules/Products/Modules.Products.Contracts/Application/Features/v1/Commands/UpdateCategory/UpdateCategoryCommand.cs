using Mediator;

namespace FSH.Modules.Products.Contracts.Application.Features.v1.Commands.UpdateCategory;

/// <summary>
/// Command to update an existing category.
/// </summary>
public sealed record UpdateCategoryCommand(int Id, string Name) : ICommand;
