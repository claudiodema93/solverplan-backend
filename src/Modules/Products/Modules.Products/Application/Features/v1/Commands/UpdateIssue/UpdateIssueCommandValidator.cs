using FluentValidation;
using FSH.Modules.Products.Contracts.Application.Features.v1.Commands.UpdateIssue;

namespace FSH.Modules.Products.Application.Features.v1.Commands.UpdateIssue;

public sealed class UpdateIssueCommandValidator : AbstractValidator<UpdateIssueCommand>
{
    public UpdateIssueCommandValidator()
    {
        RuleFor(c => c.Id)
            .GreaterThan(0).WithMessage("Id is required.");

        RuleFor(c => c.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(256).WithMessage("Title must not exceed 256 characters.");

        RuleFor(c => c.Description)
            .NotEmpty().WithMessage("Description is required.")
            .MaximumLength(4000).WithMessage("Description must not exceed 4000 characters.");

        RuleFor(c => c.Severity)
            .IsInEnum().WithMessage("Severity must be a valid enum value.");

        RuleFor(c => c.Status)
            .IsInEnum().WithMessage("Status must be a valid enum value.");

        RuleFor(c => c.ResolutionNotes)
            .MaximumLength(4000).When(c => !string.IsNullOrEmpty(c.ResolutionNotes))
            .WithMessage("ResolutionNotes must not exceed 4000 characters.");
    }
}
