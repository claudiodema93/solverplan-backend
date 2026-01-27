using FluentValidation;
using FSH.Modules.Products.Contracts.Application.Features.Commands.CreateProduct;
using FSH.Modules.Products.Contracts.v1.CreateProduct;

namespace FSH.Modules.Products.Application.Features.Commands.CreateProduct;

public sealed class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(p => p.Title)
            .NotEmpty()
            .MaximumLength(256);
    }
}
