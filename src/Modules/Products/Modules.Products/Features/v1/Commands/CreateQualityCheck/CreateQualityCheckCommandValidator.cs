using FluentValidation;
using FSH.Modules.Products.Contracts.Features.v1.Commands.CreateQualityCheck;

namespace FSH.Modules.Products.Features.v1.Commands.CreateQualityCheck;

/// <summary>
/// Validator for CreateQualityCheckCommand.
/// </summary>
public sealed class CreateQualityCheckCommandValidator : AbstractValidator<CreateQualityCheckCommand>
{
    public CreateQualityCheckCommandValidator()
    {
        RuleFor(x => x.ProductId)
            .GreaterThan(0)
            .WithMessage("Product ID must be greater than 0.");

        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Title is required.")
            .MaximumLength(200)
            .WithMessage("Title must not exceed 200 characters.");

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("Description is required.")
            .MaximumLength(2000)
            .WithMessage("Description must not exceed 2000 characters.");

        RuleFor(x => x.Department)
            .NotEmpty()
            .WithMessage("Department is required.")
            .MaximumLength(100)
            .WithMessage("Department must not exceed 100 characters.");

        RuleFor(x => x.DisplayOrder)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Display order must be 0 or greater.");
    }
}
