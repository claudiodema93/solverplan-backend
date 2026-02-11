using FluentValidation;
using FSH.Framework.Web.Validation;
using FSH.Modules.Products.Contracts.Features.v1.Queries.SearchCategories;

namespace FSH.Modules.Products.Features.v1.Queries.SearchCategories;

/// <summary>
/// Validator for SearchCategoriesQuery to ensure valid pagination and filter parameters.
/// </summary>
public sealed class SearchCategoriesQueryValidator : AbstractValidator<SearchCategoriesQuery>
{
    public SearchCategoriesQueryValidator()
    {
        Include(new PagedQueryValidator<SearchCategoriesQuery>());

        RuleFor(q => q.Search)
            .MaximumLength(128)
            .When(q => !string.IsNullOrEmpty(q.Search));
    }
}
