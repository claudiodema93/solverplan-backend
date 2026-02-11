using Mediator;

namespace FSH.Modules.Products.Contracts.Features.v1.Commands.CreateQualityCheck;

/// <summary>
/// Command to create a new quality check for a product.
/// </summary>
/// <param name="ProductId">The ID of the product this quality check belongs to.</param>
/// <param name="Title">The title or summary of the quality check.</param>
/// <param name="Description">Detailed description of the quality check.</param>
/// <param name="Department">The department responsible for the quality check.</param>
/// <param name="IsRequired">Indicates whether this quality check is mandatory.</param>
/// <param name="DisplayOrder">The display order for presenting quality checks.</param>
public sealed record CreateQualityCheckCommand(
    int ProductId,
    string Title,
    string Description,
    string Department,
    bool IsRequired = false,
    int DisplayOrder = 0) : ICommand<CreateQualityCheckResponse>;

/// <summary>
/// Response returned after successfully creating a quality check.
/// </summary>
/// <param name="Id">The unique identifier of the newly created quality check.</param>
public sealed record CreateQualityCheckResponse(int Id);
