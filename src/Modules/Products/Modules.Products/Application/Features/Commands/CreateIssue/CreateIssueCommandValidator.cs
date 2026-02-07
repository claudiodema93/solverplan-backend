using FluentValidation;
using FSH.Modules.Products.Contracts.Application.Features.Commands.CreateIssue;
using FSH.Modules.Products.Contracts.Domain.Enums;

namespace FSH.Modules.Products.Application.Features.Commands.CreateIssue;

public sealed class CreateIssueCommandValidator : AbstractValidator<CreateIssueCommand>
{
    public CreateIssueCommandValidator()
    {
        RuleFor(c => c.ProductId)
            .GreaterThan(0).WithMessage("ProductId is required.");

        RuleFor(c => c.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(256).WithMessage("Title must not exceed 256 characters.");

        RuleFor(c => c.Description)
            .NotEmpty().WithMessage("Description is required.")
            .MaximumLength(4000).WithMessage("Description must not exceed 4000 characters.");

        RuleFor(c => c.Severity)
            .IsInEnum().When(c => c.Severity.HasValue).WithMessage("Severity must be a valid enum value.");

        RuleFor(c => c.Status)
            .IsInEnum().When(c => c.Status.HasValue).WithMessage("Status must be a valid enum value.");
    }
}
