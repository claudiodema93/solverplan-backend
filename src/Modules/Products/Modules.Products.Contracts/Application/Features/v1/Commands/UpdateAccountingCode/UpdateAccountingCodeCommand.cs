using Mediator;

namespace FSH.Modules.Products.Contracts.Application.Features.v1.Commands.UpdateAccountingCode;

/// <summary>
/// Command to update an existing accounting code.
/// </summary>
/// <param name="Id">The unique identifier of the accounting code to update.</param>
/// <param name="Code">The new accounting code string. Must be unique per product.</param>
/// <param name="Description">Optional description of the accounting code purpose.</param>
public sealed record UpdateAccountingCodeCommand(
    int Id,
    string Code,
    string? Description = null) : ICommand;
