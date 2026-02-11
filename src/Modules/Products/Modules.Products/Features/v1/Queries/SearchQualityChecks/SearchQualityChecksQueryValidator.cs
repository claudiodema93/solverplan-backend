using FluentValidation;
using FSH.Modules.Products.Contracts.Features.v1.Queries.SearchQualityChecks;

namespace FSH.Modules.Products.Features.v1.Queries.SearchQualityChecks;

/// <summary>
/// Validator for SearchQualityChecksQuery.
/// </summary>
public sealed class SearchQualityChecksQueryValidator : AbstractValidator<SearchQualityChecksQuery>
{
    public SearchQualityChecksQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(1)
            .When(x => x.PageNumber.HasValue)
            .WithMessage("Page number must be at least 1.");

        RuleFor(x => x.PageSize)
            .GreaterThan(0)
            .LessThanOrEqualTo(100)
            .When(x => x.PageSize.HasValue)
            .WithMessage("Page size must be between 1 and 100.");
    }
}
