using FSH.Modules.Products.Contracts.Domain.DTOs;
using Mediator;

namespace FSH.Modules.Products.Contracts.Features.v1.Queries.GetQualityCheckById;

/// <summary>
/// Query to retrieve a quality check by its unique identifier.
/// </summary>
/// <param name="Id">The unique identifier of the quality check to retrieve.</param>
public sealed record GetQualityCheckByIdQuery(int Id) : IQuery<QualityCheckDto?>;
