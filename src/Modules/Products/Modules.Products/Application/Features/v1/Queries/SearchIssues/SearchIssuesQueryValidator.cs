using FluentValidation;
using FSH.Framework.Web.Validation;
using FSH.Modules.Products.Contracts.Application.Features.v1.Queries.SearchIssues;

namespace FSH.Modules.Products.Application.Features.v1.Queries.SearchIssues;

/// <summary>
/// Validator for SearchIssuesQuery to ensure valid pagination and filter parameters.
/// </summary>
public sealed class SearchIssuesQueryValidator : AbstractValidator<SearchIssuesQuery>
{
    public SearchIssuesQueryValidator()
    {
        Include(new PagedQueryValidator<SearchIssuesQuery>());

        RuleFor(q => q.Search)
            .MaximumLength(256)
            .When(q => !string.IsNullOrEmpty(q.Search));

        RuleFor(q => q.ProductId)
            .GreaterThan(0)
            .When(q => q.ProductId.HasValue);

        RuleFor(q => q.Severity)
            .IsInEnum()
            .When(q => q.Severity.HasValue);

        RuleFor(q => q.Status)
            .IsInEnum()
            .When(q => q.Status.HasValue);
    }
}
