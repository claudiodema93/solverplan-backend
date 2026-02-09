using FluentValidation;
using FSH.Modules.Products.Contracts.Application.Features.v1.Commands.UpdateQualityCheck;

namespace FSH.Modules.Products.Application.Features.v1.Commands.UpdateQualityCheck;

/// <summary>
/// Validator for UpdateQualityCheckCommand.
/// </summary>
public sealed class UpdateQualityCheckCommandValidator : AbstractValidator<UpdateQualityCheckCommand>
{
    public UpdateQualityCheckCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("Quality check ID must be greater than 0.");

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
