using FSH.Modules.Products.Contracts.DTOs;
using Mediator;

namespace FSH.Modules.Products.Contracts.Application.Features.Queries.GetIssueById;

/// <summary>
/// Query to retrieve an issue by its unique identifier.
/// </summary>
public sealed record GetIssueByIdQuery(int Id) : IQuery<IssueDto?>;
