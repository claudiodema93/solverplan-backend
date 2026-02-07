using FluentValidation;
using FSH.Modules.Products.Contracts.Application.Features.v1.Commands.UpdateCategory;

namespace FSH.Modules.Products.Application.Features.v1.Commands.UpdateCategory;

public sealed class UpdateCategoryCommandValidator : AbstractValidator<UpdateCategoryCommand>
{
    public UpdateCategoryCommandValidator()
    {
        RuleFor(c => c.Id)
            .GreaterThan(0).WithMessage("Category Id must be greater than 0.");

        RuleFor(c => c.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(128).WithMessage("Name must not exceed 128 characters.");
    }
}
