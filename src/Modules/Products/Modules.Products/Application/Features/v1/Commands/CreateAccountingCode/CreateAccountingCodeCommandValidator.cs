using FluentValidation;
using FSH.Modules.Products.Contracts.Application.Features.v1.Commands.CreateAccountingCode;

namespace FSH.Modules.Products.Application.Features.v1.Commands.CreateAccountingCode;

public sealed class CreateAccountingCodeCommandValidator : AbstractValidator<CreateAccountingCodeCommand>
{
    public CreateAccountingCodeCommandValidator()
    {
        RuleFor(x => x.ProductId)
            .GreaterThan(0).WithMessage("ProductId must be greater than 0.");

        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Code is required.")
            .MaximumLength(64).WithMessage("Code must not exceed 64 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(512).WithMessage("Description must not exceed 512 characters.")
            .When(x => !string.IsNullOrEmpty(x.Description));
    }
}
