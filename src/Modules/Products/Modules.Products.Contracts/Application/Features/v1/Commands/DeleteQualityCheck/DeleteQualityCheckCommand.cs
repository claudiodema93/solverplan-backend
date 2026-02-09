using Mediator;

namespace FSH.Modules.Products.Contracts.Application.Features.v1.Commands.DeleteQualityCheck;

/// <summary>
/// Command to delete a quality check by ID.
/// </summary>
/// <param name="Id">The unique identifier of the quality check to delete.</param>
public sealed record DeleteQualityCheckCommand(int Id) : ICommand;
