using FSH.Modules.Products.Contracts.DTOs;
using Mediator;

namespace FSH.Modules.Products.Contracts.Features.v1.Queries.GetIssueById;

/// <summary>
/// Query to retrieve an issue by its unique identifier.
/// </summary>
/// <param name="Id">The unique identifier of the issue to retrieve.</param>
public sealed record GetIssueByIdQuery(int Id) : IQuery<IssueDto?>;
