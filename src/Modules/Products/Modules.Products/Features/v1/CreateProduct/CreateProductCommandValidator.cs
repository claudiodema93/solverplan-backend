using FluentValidation;
using FSH.Modules.Products.Contracts.v1.CreateProduct;

namespace FSH.Modules.Products.Features.v1.CreateProduct;

public sealed class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(p => p.Title)
            .NotEmpty()
            .MaximumLength(256);

        RuleFor(p => p.Revision)
            .NotEmpty()
            .MaximumLength(64);
    }
}
