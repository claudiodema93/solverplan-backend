using Mediator;

namespace FSH.Modules.Products.Contracts.Application.Features.v1.Commands.DeleteCategory;

/// <summary>
/// Command to delete a category.
/// </summary>
public sealed record DeleteCategoryCommand(int Id) : ICommand;
