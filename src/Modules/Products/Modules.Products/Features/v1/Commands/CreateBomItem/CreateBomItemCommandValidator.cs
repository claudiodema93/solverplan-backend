using FluentValidation;
using FSH.Modules.Products.Contracts.Features.v1.Commands.CreateBomItem;

namespace FSH.Modules.Products.Features.v1.Commands.CreateBomItem;

public sealed class CreateBomItemCommandValidator : AbstractValidator<CreateBomItemCommand>
{
    public CreateBomItemCommandValidator()
    {
        RuleFor(x => x.ProductId)
            .GreaterThan(0).WithMessage("ProductId must be greater than 0.");

        RuleFor(x => x.ChildProductId)
            .GreaterThan(0).WithMessage("ChildProductId must be greater than 0.")
            .NotEqual(x => x.ProductId).WithMessage("ChildProductId must be different from ProductId. A product cannot reference itself in a BOM.");

        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Quantity must be greater than 0.");

        RuleFor(x => x.Notes)
            .MaximumLength(2000).WithMessage("Notes must not exceed 2000 characters.")
            .When(x => !string.IsNullOrEmpty(x.Notes));
    }
}
