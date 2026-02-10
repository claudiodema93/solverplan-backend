using FluentValidation;
using FSH.Modules.Products.Contracts.Application.Features.v1.Commands.UpdateBomItem;

namespace FSH.Modules.Products.Application.Features.v1.Commands.UpdateBomItem;

public sealed class UpdateBomItemCommandValidator : AbstractValidator<UpdateBomItemCommand>
{
    public UpdateBomItemCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Id must be greater than 0.");

        RuleFor(x => x.ChildProductId)
            .GreaterThan(0).WithMessage("ChildProductId must be greater than 0.");

        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Quantity must be greater than 0.");

        RuleFor(x => x.Notes)
            .MaximumLength(2000).WithMessage("Notes must not exceed 2000 characters.")
            .When(x => !string.IsNullOrEmpty(x.Notes));
    }
}
