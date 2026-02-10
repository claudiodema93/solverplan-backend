using FluentValidation;
using FSH.Modules.Products.Contracts.Application.Features.v1.Queries.SearchAccountingCodes;

namespace FSH.Modules.Products.Application.Features.v1.Queries.SearchAccountingCodes;

public sealed class SearchAccountingCodesQueryValidator : AbstractValidator<SearchAccountingCodesQuery>
{
    public SearchAccountingCodesQueryValidator()
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
    }
}
