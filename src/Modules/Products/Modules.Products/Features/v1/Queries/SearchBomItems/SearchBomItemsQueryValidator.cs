using FluentValidation;
using FSH.Modules.Products.Contracts.Features.v1.Queries.SearchBomItems;

namespace FSH.Modules.Products.Features.v1.Queries.SearchBomItems;

public sealed class SearchBomItemsQueryValidator : AbstractValidator<SearchBomItemsQuery>
{
    public SearchBomItemsQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(1).WithMessage("PageNumber must be greater than or equal to 1.")
            .When(x => x.PageNumber.HasValue);

        RuleFor(x => x.PageSize)
            .GreaterThan(0).WithMessage("PageSize must be greater than 0.")
            .LessThanOrEqualTo(200).WithMessage("PageSize must not exceed 200.")
            .When(x => x.PageSize.HasValue);

        RuleFor(x => x.ProductId)
            .GreaterThan(0).WithMessage("ProductId must be greater than 0.")
            .When(x => x.ProductId.HasValue);

        RuleFor(x => x.ChildProductId)
            .GreaterThan(0).WithMessage("ChildProductId must be greater than 0.")
            .When(x => x.ChildProductId.HasValue);
    }
}
