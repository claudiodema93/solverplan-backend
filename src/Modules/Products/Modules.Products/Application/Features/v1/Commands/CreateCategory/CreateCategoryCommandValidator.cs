using FluentValidation;
using FSH.Modules.Products.Contracts.Application.Features.v1.Commands.CreateCategory;

namespace FSH.Modules.Products.Application.Features.v1.Commands.CreateCategory;

public sealed class CreateCategoryCommandValidator : AbstractValidator<CreateCategoryCommand>
{
    public CreateCategoryCommandValidator()
    {
        RuleFor(c => c.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(128).WithMessage("Name must not exceed 128 characters.");
    }
}
