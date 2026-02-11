using Mediator;

namespace FSH.Modules.Products.Contracts.Features.v1.Commands.CreateAccountingCode;

/// <summary>
/// Command to create a new accounting code for a product.
/// </summary>
/// <param name="ProductId">The identifier of the product to associate the accounting code with.</param>
/// <param name="Code">The accounting code string (e.g., "MAT-100", "LABOR-001"). Must be unique per product.</param>
/// <param name="Description">Optional description of the accounting code purpose.</param>
public sealed record CreateAccountingCodeCommand(
    int ProductId,
    string Code,
    string? Description = null) : ICommand<int>;
