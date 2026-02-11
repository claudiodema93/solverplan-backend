using Mediator;

namespace FSH.Modules.Products.Contracts.Features.v1.Commands.UpdateQualityCheck;

/// <summary>
/// Command to update an existing quality check.
/// </summary>
/// <param name="Id">The unique identifier of the quality check to update.</param>
/// <param name="ProductId">The ID of the product this quality check belongs to.</param>
/// <param name="Title">The title or summary of the quality check.</param>
/// <param name="Description">Detailed description of the quality check.</param>
/// <param name="Department">The department responsible for the quality check.</param>
/// <param name="IsRequired">Indicates whether this quality check is mandatory.</param>
/// <param name="DisplayOrder">The display order for presenting quality checks.</param>
public sealed record UpdateQualityCheckCommand(
    int Id,
    int ProductId,
    string Title,
    string Description,
    string Department,
    bool IsRequired = false,
    int DisplayOrder = 0) : ICommand;
