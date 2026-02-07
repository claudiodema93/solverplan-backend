using FluentValidation;
using FSH.Framework.Web.Validation;
using FSH.Modules.Products.Contracts.Application.Features.v1.Queries.SearchProducts;

namespace FSH.Modules.Products.Application.Features.v1.Queries.SearchProducts;

/// <summary>
/// Validator for SearchProductsQuery to ensure valid pagination and filter parameters.
/// </summary>
public sealed class SearchProductsQueryValidator : AbstractValidator<SearchProductsQuery>
{
    public SearchProductsQueryValidator()
    {
        Include(new PagedQueryValidator<SearchProductsQuery>());

        RuleFor(q => q.Search)
            .MaximumLength(500)
            .When(q => !string.IsNullOrEmpty(q.Search));

        RuleFor(q => q.CategoryId)
            .GreaterThan(0)
            .When(q => q.CategoryId.HasValue);
    }
}
