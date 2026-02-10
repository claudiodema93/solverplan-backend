using FSH.Modules.Products.Contracts.DTOs;
using Mediator;

namespace FSH.Modules.Products.Contracts.Application.Features.v1.Queries.GetAccountingCodeById;

/// <summary>
/// Query to retrieve an accounting code by its unique identifier.
/// </summary>
/// <param name="Id">The unique identifier of the accounting code.</param>
public sealed record GetAccountingCodeByIdQuery(int Id) : IQuery<AccountingCodeDto?>;
