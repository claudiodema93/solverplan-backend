using Mediator;

namespace FSH.Modules.Products.Contracts.Application.Features.v1.Commands.DeleteAccountingCode;

/// <summary>
/// Command to delete an existing accounting code.
/// </summary>
/// <param name="Id">The unique identifier of the accounting code to delete.</param>
public sealed record DeleteAccountingCodeCommand(int Id) : ICommand;
